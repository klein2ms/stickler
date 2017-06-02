using System;
using System.Collections.Generic;
using System.Linq;

namespace Stickler.Engine
{
    public class DefaultCompiledRuleStore : ICompiledRuleStore
    {
        private readonly ILexer _lexer;
        private readonly IParser _parser;
        private readonly IInterpreter _interpreter;
        private readonly IRuleStore _ruleStore;

        public DefaultCompiledRuleStore()
            : this(new DefaultRuleStore())
        {
        }

        public DefaultCompiledRuleStore(IRuleStore ruleStore)
            : this(new DefaultLexer(), new DefaultParser(), new DefaultInterpreter(), ruleStore)
        {
        }

        public DefaultCompiledRuleStore(
            ILexer lexer,
            IParser parser,
            IInterpreter interpreter,
            IRuleStore ruleStore)
        {
            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            if (ruleStore == null)
                throw new ArgumentNullException(nameof(ruleStore));

            _lexer = lexer;
            _parser = parser;
            _interpreter = interpreter;
            _ruleStore = ruleStore;
        }

        public IEnumerable<Rule<TTarget, TComparison>> GetRules<TTarget, TComparison>()
        {
            var ruleDtos = _ruleStore.GetRules<TTarget, TComparison>();

            var targetTypeName = typeof(TTarget).Name;

            var matchingRecords = ruleDtos
                .Where(r => string.Equals(r.TargetTypeName, targetTypeName, StringComparison.CurrentCultureIgnoreCase));

            var targetRuleDefinitions = matchingRecords.Select(GetRuleDefinition);

            var applicableRulesDefinitions =
                targetRuleDefinitions.Where(
                    r => r.RuleConditions.All(
                        rc => string.Equals(typeof(TComparison).Name, rc.ComparisonTypeName, StringComparison.CurrentCultureIgnoreCase)
                              || rc.ComparisonType == RuleObject.Constant));

            return applicableRulesDefinitions.Join(matchingRecords,
                def => def.Id,
                dto => dto.Id,
                (def, dto) => new { RuleDefinition = def, RuleDto = dto })
                .Select(rule => new Rule<TTarget, TComparison>
                {
                    Id = rule.RuleDto.Id,
                    Name = rule.RuleDto.Name,
                    Expression = rule.RuleDto.RuleExpression,
                    Validate = _interpreter.Interpret<TTarget, TComparison>(rule.RuleDefinition)
                })
                .ToList();
        }

        public Rule<TTarget, TComparison> AddRule<TTarget, TComparison>(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            var newRule = _ruleStore.AddRule<TTarget, TComparison>(ruleDto);

            return GetRule<TTarget, TComparison>(newRule);
        }

        public Rule<TTarget, TComparison> UpdateRule<TTarget, TComparison>(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));
            
            var updatedRule = _ruleStore.UpdateRule<TTarget, TComparison>(ruleDto);

            return GetRule<TTarget, TComparison>(updatedRule);
        }

        public void DeleteRule(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));
            
            _ruleStore.DeleteRule(ruleDto);
        }

        private RuleDefinition GetRuleDefinition(RuleDto ruleDto)
        {
            var tokens = _lexer.Lex(ruleDto.RuleExpression).ToList();
            var ruleDefinition = _parser.Parse(tokens);
            ruleDefinition.Id = ruleDto.Id;

            return ruleDefinition;
        }

        private Rule<TTarget, TComparison> GetRule<TTarget, TComparison>(RuleDto ruleDto)
        {
            var ruleDefinition = GetRuleDefinition(ruleDto);

            return new Rule<TTarget, TComparison>
            {
                Id = ruleDto.Id,
                Name = ruleDto.Name,
                Expression = ruleDto.RuleExpression,
                Validate = _interpreter.Interpret<TTarget, TComparison>(ruleDefinition)
            };
        }
    }
}
