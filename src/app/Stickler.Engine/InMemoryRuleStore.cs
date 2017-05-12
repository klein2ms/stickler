using System;
using System.Collections.Generic;
using System.Linq;

namespace Stickler.Engine
{
    public class InMemoryRuleStore : IRuleStore
    {
        private readonly IParser _parser;
        private readonly ILexer _lexer;
        private readonly IInterpreter _interpreter;
        private readonly IDictionary<string, RuleDto> _ruleRecords;

        public InMemoryRuleStore(IParser parser, ILexer lexer, IInterpreter interpreter)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (lexer == null)
                throw new ArgumentNullException(nameof(lexer));

            if (interpreter == null)
                throw new ArgumentNullException(nameof(interpreter));

            _parser = parser;
            _lexer = lexer;
            _interpreter = interpreter;
            _ruleRecords = new Dictionary<string, RuleDto>();
        }

        public IEnumerable<Rule<TTarget, TComparison>> GetRules<TTarget, TComparison>()
        {
            var targetTypeName = typeof(TTarget).Name;
            
            var matchingRecords = _ruleRecords
                .Values
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
                (def, dto) => new {RuleDefinition = def, RuleDto = dto})
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

            var toAdd = ruleDto.Clone();

            if (string.IsNullOrWhiteSpace(ruleDto.Id))
                toAdd.Id = Guid.NewGuid().ToString();

            _ruleRecords.Add(toAdd.Id, toAdd);

            return GetRule<TTarget, TComparison>(toAdd);
        }

        public Rule<TTarget, TComparison> UpdateRule<TTarget, TComparison>(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            var toUpdate = ruleDto.Clone();

            if (!_ruleRecords.ContainsKey(toUpdate.Id))
                throw new ArgumentException($"The key {toUpdate.Id} was not found.");

            DeleteRule(toUpdate);
            return AddRule<TTarget, TComparison>(toUpdate);
        }

        public void DeleteRule(RuleDto ruleDto)
        {
            if (ruleDto == null)
                throw new ArgumentNullException(nameof(ruleDto));

            var toDelete = ruleDto.Clone();

            _ruleRecords.Remove(toDelete.Id);
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
