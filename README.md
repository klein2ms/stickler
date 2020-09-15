![Stickler Logo](/docs/images/stickler-letters.png "Stickler Logo")

> When you're a stickler for the rules.

## Example Rule Service Architecture

![Example Service Architecture](/docs/images/rule-service-example.png "Example Service Architecture")

---

## Stickler.Engine Architecture

![Class Diagram](/docs/images/rules-engine-class-diagram.png "Class Diagram")

---

## Example Usage

```csharp
Stickler.Instance
        .Enforce(new RuleBookBuilder<Holding, Fund>()
            .AddRule(new RuleDto
            {
                Name = "Alert Rule",
                TargetTypeName = "Holding",
                RuleExpression = "ensure holding.Nav is >= fund.Nav"
            })
            .OnSuccess((target, comparison) =>
            {
                Console.WriteLine($"Succeeded for {target} and {comparison}");
            })
            .OnFailure((target, comparison) =>
            {
                Console.WriteLine($"Failed for {target} and {comparison}");
            })
            .Build())
        .Enforce(new RuleBookBuilder<Fund, Holding>()
            .AddRule(new RuleDto
            {
                Name = "Function Rule",
                TargetTypeName = "Fund",
                RuleExpression = "ensure fund.GetNav() is > holding.Nav"
            })
            .OnSuccess((target, comparison) => target.Nav + comparison.Nav)
            .Build());

var holding = new Holding { Nav = 25 };
var fund = new Fund { Nav = 35 };

var result = Stickler.Instance.Handle<Fund, Holding, decimal>(fund, holding);

Assert.That(result, Is.EqualTo(holding.Nav + fund.Nav));
```
