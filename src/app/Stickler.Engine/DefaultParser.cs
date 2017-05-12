using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stickler.Engine
{
    public class DefaultParser
    {
        private Queue<RuleToken> _tokenSequence;
        private RuleToken _previousToken;
        private RuleToken _currentToken;
        private RuleToken _nextToken;
        
        public RuleDefinition Parse(IList<RuleToken> tokens)
        {
            CheckForInvalidTokens(tokens);
            LoadTokenSequence(tokens);
            SetupPointerTokens();

            var ruleDefinition = new RuleDefinition();

            AddTarget(ruleDefinition);
            AddRuleCondition(ruleDefinition);

            DiscardToken(Lexicon.SequenceTerminator);

            return ruleDefinition;
        }

        private static void CheckForInvalidTokens(IList<RuleToken> tokens)
        {
            if (tokens.All(t => t.Lexicon != Lexicon.Invalid))
                return;
            {
                var invalidTokens = tokens
                    .Where(t => t.Lexicon == Lexicon.Invalid)
                    .Select(t => t.Lexicon);

                throw new ParserException($"Rule contains the following invalid tokens: {string.Join(", ", invalidTokens)}");
            }
        }

        private void AddTarget(RuleDefinition ruleDefinition)
        {
            DiscardToken(Lexicon.Ensure);

            switch (_currentToken.Lexicon)
            {
                case Lexicon.Object:
                    ruleDefinition.TargetType = RuleObject.Object;

                    var objectArgs = _currentToken.Value.Split('.');
                    ruleDefinition.TargetTypeName = objectArgs[0];
                    ruleDefinition.TargetAttribute = objectArgs[1];

                    DiscardToken(Lexicon.Object);
                    break;
                default:
                    throw new ParserException(GetExpectedTokenExceptionMessage(Lexicon.Object, _previousToken.Lexicon, _currentToken.Lexicon));
            }
           
            DiscardToken(Lexicon.Is);
        }

        private void AddRuleCondition(RuleDefinition ruleDefinition)
        {
            var ruleCondition = new RuleCondition();

            AddComparatorOperator(ruleCondition);
            AddComputationOperator(ruleCondition);
            AddComparison(ruleCondition);
            AddLogicalOperator(ruleCondition);
            
            ruleDefinition.RuleConditions.Add(ruleCondition);

            if (ruleCondition.LogicalOperator != RuleLogicalOperator.None)
                AddRuleCondition(ruleDefinition);
        }

        private void AddComparatorOperator(RuleCondition ruleCondition)
        {
            switch (_currentToken.Lexicon)
            {
                case Lexicon.Within:
                    DiscardToken(Lexicon.Within);
                    DiscardToken(Lexicon.Number);

                    ruleCondition.Operator = RuleComparatorOperator.Within;
                    ruleCondition.Value = _previousToken.Value;
                    
                    DiscardToken(Lexicon.Of);
                    break;
                case Lexicon.GreaterThan:
                    ruleCondition.Operator = RuleComparatorOperator.GreaterThan;
                    DiscardToken(Lexicon.GreaterThan);
                    break;
                case Lexicon.GreaterThanOrEqualTo:
                    ruleCondition.Operator = RuleComparatorOperator.GreaterThanOrEqualTo;
                    DiscardToken(Lexicon.GreaterThanOrEqualTo);
                    break;
                case Lexicon.LessThan:
                    ruleCondition.Operator = RuleComparatorOperator.LessThan;
                    DiscardToken(Lexicon.LessThan);
                    break;
                case Lexicon.LessThanOrEqualTo:
                    ruleCondition.Operator = RuleComparatorOperator.LessThan;
                    DiscardToken(Lexicon.LessThanOrEqualTo);
                    break;
                case Lexicon.EqualTo:
                    ruleCondition.Operator = RuleComparatorOperator.EqualTo;
                    DiscardToken(Lexicon.EqualTo);
                    break;
            }
        }

        private void AddComputationOperator(RuleCondition ruleCondition)
        {
            if (_currentToken.Lexicon != Lexicon.The)
            {
                ruleCondition.Computation = RuleComputationalOperator.None;
                return;
            }

            DiscardToken(Lexicon.The);
            
            switch (_currentToken.Lexicon)
            {
                case Lexicon.Average:
                    ruleCondition.Computation = RuleComputationalOperator.Average;
                    DiscardToken(Lexicon.Average);
                    break;
                case Lexicon.Sum:
                    ruleCondition.Computation = RuleComputationalOperator.Sum;
                    DiscardToken(Lexicon.Sum);
                    break;
                default:
                    throw new ParserException(
                        GetExpectedTokenExceptionMessage(new[] { Lexicon.Average, Lexicon.Sum }, _previousToken.Lexicon, _currentToken.Lexicon));
            }

            DiscardToken(Lexicon.For);
        }

        private void AddComparison(RuleCondition ruleCondition)
        {
            switch (_currentToken.Lexicon)
            {
                case Lexicon.Object:
                    ruleCondition.ComparisonType = RuleObject.Object;

                    var objectArgs = _currentToken.Value.Split('.');
                    ruleCondition.ComparisonTypeName = objectArgs[0];
                    ruleCondition.ComparisonAttribute = objectArgs[1];
                    
                    DiscardToken(Lexicon.Object);
                    break;
                case Lexicon.String:
                    ruleCondition.ComparisonType = RuleObject.Constant;
                    ruleCondition.ComparisonTypeName = "string";
                    ruleCondition.Value = _currentToken.Value;
                    DiscardToken(Lexicon.String);
                    break;
                case Lexicon.Number:
                    ruleCondition.ComparisonType = RuleObject.Constant;
                    ruleCondition.ComparisonTypeName = "number";
                    ruleCondition.Value = _currentToken.Value;
                    DiscardToken(Lexicon.Number);
                    break;
                case Lexicon.Date:
                    ruleCondition.ComparisonType = RuleObject.Constant;
                    ruleCondition.ComparisonTypeName = "date";
                    ruleCondition.Value = _currentToken.Value;
                    DiscardToken(Lexicon.Date);
                    break;
                default:
                    throw new ParserException(
                        GetExpectedTokenExceptionMessage(new [] {Lexicon.Object, Lexicon.String, Lexicon.Number, Lexicon.Date}, _previousToken.Lexicon, _currentToken.Lexicon));
            }
        }

        private void AddLogicalOperator(RuleCondition ruleCondition)
        {
            switch (_currentToken.Lexicon)
            {
                case Lexicon.And:
                    ruleCondition.LogicalOperator = RuleLogicalOperator.And;
                    DiscardToken(Lexicon.And);
                    break;
                case Lexicon.Or:
                    ruleCondition.LogicalOperator = RuleLogicalOperator.Or;
                    DiscardToken(Lexicon.Or);
                    break;
                default:
                    ruleCondition.LogicalOperator = RuleLogicalOperator.None;
                    break;
            }
        }

        private static string GetExpectedTokenExceptionMessage(Lexicon expected, Lexicon previous, Lexicon actual)
        {
            return $"Expected to find {expected} after {previous} but found: {actual}";
        }

        private static string GetExpectedTokenExceptionMessage(IEnumerable expected, Lexicon previous, Lexicon actual)
        {
            return $"Expected to find {string.Join(", ", expected)} after {previous} but found: {actual}";
        }

        private void LoadTokenSequence(IEnumerable<RuleToken> tokens)
        {
            _tokenSequence = new Queue<RuleToken>();

            foreach (var token in tokens)
            {
                _tokenSequence.Enqueue(token);
            }
        }

        private void SetupPointerTokens()
        {
            _previousToken = null;
            _currentToken = _tokenSequence.Dequeue();
            _nextToken = _tokenSequence.Dequeue();
        }

        private void CheckThatCurrentTokenIs(Lexicon lexicon)
        {
            if (_currentToken.Lexicon != lexicon)
                throw new ParserException(GetExpectedTokenExceptionMessage(lexicon, _previousToken.Lexicon, _currentToken.Lexicon));
        }

        private void DiscardToken(Lexicon lexicon)
        {
            CheckThatCurrentTokenIs(lexicon);
            DiscardToken();
        }

        private void DiscardToken()
        {
            _previousToken = _currentToken.Clone();
            _currentToken = _nextToken.Clone();

            _nextToken = _tokenSequence.Any() ?
                _tokenSequence.Dequeue() : new RuleToken(Lexicon.SequenceTerminator, string.Empty);
        }
    }
}
