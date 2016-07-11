using ExpressionEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDrivenBot.Model
{
    [Serializable]
    public partial class Condition : Step
    {
        public override Step GetNextStep(string result = null, Dictionary<string, string> properties = null)
        {
            string expression = InjectPropertyValues(Expression, properties);

            CompiledExpression<bool> compiledExpression = new CompiledExpression<bool>(expression);

            if (compiledExpression.Eval())
                return NextStepWhenTrue;
            else
                return NextStepWhenFalse;
        }
    }
}
