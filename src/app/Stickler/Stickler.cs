using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Stickler.Engine;

namespace Stickler
{
    public sealed class Stickler
    {
        private static readonly Lazy<Stickler> _lazy 
            = new Lazy<Stickler>(() => new Stickler());

        public static Stickler Instance => _lazy.Value;
        
        private readonly IDictionary<KeyValuePair<Type, Type>, dynamic> _ruleBooks;
        
        private Stickler()
        {
            _ruleBooks = new ConcurrentDictionary<KeyValuePair<Type, Type>, dynamic>();
        }
        
        public Stickler Enforce<TTarget, TComparison>(RuleBook<TTarget, TComparison> ruleBook)
        {
            if (ruleBook == null)
                throw new ArgumentNullException(nameof(ruleBook));

            var key = new KeyValuePair<Type, Type>(typeof(TTarget), typeof(TComparison));

            if (_ruleBooks.ContainsKey(key))
                throw new ArgumentException($"Rule Book already exists for type: {typeof(TTarget)} with a comparison of type: {typeof(TComparison)}");

            _ruleBooks.Add(key, ruleBook);

            return Instance;
        }
        
        public void Handle<TTarget, TComparison>(TTarget target, TComparison comparison)
        {
            var ruleBook = GetRuleBook<TTarget, TComparison>();
            var resultSet = ruleBook.Evaluate(target, comparison);
            
            switch (resultSet.Status)
            {
                case ResultStatus.Invalid:
                    break;
                case ResultStatus.Pass:
                    ruleBook.OnSuccess(target, comparison);
                    break;
                case ResultStatus.Fail:
                    ruleBook.OnFailure(target, comparison);
                    break;
                case ResultStatus.Ignored:
                    ruleBook.OnIgnored(target, comparison);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public TResult Handle<TTarget, TComparison, TResult>(TTarget target, TComparison comparison)
        {
            var ruleBook = GetRuleBook<TTarget, TComparison>();
            var resultSet = ruleBook.Evaluate(target, comparison);

            switch (resultSet.Status)
            {
                case ResultStatus.Invalid:
                    return default(TResult);
                case ResultStatus.Pass:
                    return ruleBook.OnSuccess(target, comparison);
                case ResultStatus.Fail:
                    return ruleBook.OnFailure(target, comparison);
                case ResultStatus.Ignored:
                    return ruleBook.OnIgnored(target, comparison);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private RuleBook<TTarget, TComparison> GetRuleBook<TTarget, TComparison>()
        {
            var key = new KeyValuePair<Type, Type>(typeof(TTarget), typeof(TComparison));

            if (!_ruleBooks.ContainsKey(key))
                throw new ArgumentOutOfRangeException(
                    $"Rule Book not defined for type {typeof(TTarget)} with comparison type of {typeof(TComparison)}");

            dynamic ruleBookDynamic;

            _ruleBooks.TryGetValue(key, out ruleBookDynamic);

            var ruleBook = ruleBookDynamic as RuleBook<TTarget, TComparison>;

            if (ruleBook == null)
                throw new ArgumentException();

            return ruleBook;
        }
    }
}
