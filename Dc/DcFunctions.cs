using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dc
{
    public class DcFunctions
    {
        public Dictionary<string, DcFunction> All { get; }

        public double Execute(string name, params double[] p)
        {
            if (!All.ContainsKey(name))
                throw new Exception("Function unknown: " + name);

            return All[name].Execute(p);
        }

        public void Execute(string name, Stack<double> stack)
        {
            if (!All.ContainsKey(name))
                throw new Exception("Function unknown: " + name);
            var func = All[name];
            if (stack.Count < func.ParameterCount)
                throw new Exception("Stack underflow");
            var p = Enumerable.Range(0, func.ParameterCount).Select(i => stack.Pop()).Reverse().ToArray();
            stack.Push(func.Execute(p));
        }

        public DcFunctions()
        {
            All = DcFunction.GetAll(typeof(Math)).ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);
        }
    }

    [DebuggerDisplay("{Name}(double[{ParameterCount}])")]
    public class DcFunction
    {
        public DcFunction(string name, int parameterCount, Func<double[], double> function)
        {
            Name = name;
            ParameterCount = parameterCount;
            Function = function;
        }

        public string Name { get; }
        public int ParameterCount { get; }
        Func<double[], double> Function { get; }

        public double Execute(params double[] p)
        {
            return Function(p);
        }

        public static List<DcFunction> GetAll(Type type)
        {
            var result = new List<DcFunction>();

            // Get methods taking only doubles and returning double

            var functions = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.ReturnType == typeof(double))
                .Where(m => m.GetParameters().All(pi => pi.ParameterType == typeof(double)))
                .GroupBy(m => m.Name)
                .Select(g => g.OrderBy(m => m.GetParameters().Length).First());

            foreach (var func in functions)
                result.Add(new DcFunction(func.Name, func.GetParameters().Length, p => Caller(func, p)));

            // Get all double fields

            result.AddRange(type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(double))
                .Where(f => !result.Any(dc => String.Equals(f.Name, dc.Name, StringComparison.OrdinalIgnoreCase)))
                .Select(f => new DcFunction(f.Name, 0, a => (double)f.GetValue(null))));

            return result;
        }

        static double Caller(MethodInfo method, double[] p)
        {
            if (method.GetParameters().Length != p.Length)
                throw new Exception("Parameter count mismatch");
            return (double)method.Invoke(null, p.Cast<object>().ToArray());
        }
    }
}
