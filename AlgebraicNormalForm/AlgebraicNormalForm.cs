using System.Text;

namespace AlgebraicNormalForm;

public static class AlgebraicNormalForm
{
    private static IEnumerable<bool> ParseEval(string eval)
    {
        return eval.Select(c => c switch
        {
            '1' => true,
            '0' => false,
            _ => throw new ArgumentException($"Eval can only contain 1's and 0's! Encountered: '{c}'.")
        });
    }
    
    private static string Input(string message)
    {
        Console.Write(message);
        return Console.ReadLine() ?? "";
    }

    private static bool[] ReadEval()
    {
        string eval = Input("Eval >> ");

        if ((eval.Length & eval.Length - 1) != 0)
            throw new ArgumentException($"Eval length should be a power of two, was {eval.Length}!");
        if (eval.Length <= 0)
            throw new ArgumentException($"Eval length should be positive! Was {eval.Length}.");

        return ParseEval(eval).ToArray();
    }

    private static void Intro()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("== Algebraic normal form finder ==");
        Console.WriteLine("This program finds the algebraic normal form of a function by a given eval.");
        Console.WriteLine("Specify the eval of your function (e.g. 10010100):");
        Console.WriteLine();
        Console.ResetColor();
    }
    
    public static void Main()
    {
        Intro();
        
        Start:
        bool[] eval;
        try
        {
            eval = ReadEval();
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.Message);
            Console.ResetColor();
            goto Start;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(AnfToString(Q_GetAnfCoefficients(eval)));
        Console.WriteLine();
        Console.ResetColor();
        
        goto Start;
    }

    private static bool[] GetAnfCoefficients(bool[] eval)
    {
        bool[] result = eval.ToArray();

        for (int i = 0; i < result.Length; i++)
        for (int j = i + 1; j < result.Length; j++)
            if ((j & i) != 0)
                result[j] ^= result[i];

        return result;
    }

    private static bool[] Q_GetAnfCoefficients(bool[] eval)
    {
        bool[] result = eval.ToArray();

        for (int i = 0; i < result.Length; i++)
            if (result[i])
                for (int j = i + 1; j < result.Length; j++)
                    if ((j & i) == i)
                        result[j] ^= result[i];
        return result;
    }

    private static string AnfToString(bool[] coefficients)
    {
        StringBuilder resultBuilder = new();

        if (coefficients.All(v => !v))
            return "0";
        
        if (coefficients[0] && coefficients.Skip(1).All(v => !v))
            return "1";
        
        if (coefficients[0])
            resultBuilder.Append("1 + ");
        
        int countVariables = (int)Math.Log2(coefficients.Length);
        for (int i = 1; i < coefficients.Length; i++)
        {
            if (!coefficients[i]) 
                continue;
            
            string str = Convert.ToString(i, 2);
            for (int j = 0; j < str.Length; j++)
            {
                if (str[^(j + 1)] == '1')
                {
                    string index = new((countVariables - j).ToString().Reverse().ToArray());
                    resultBuilder.Append(index);
                    resultBuilder.Append('x');
                }
            }
            resultBuilder.Append(" + ");
        }

        resultBuilder.Remove(resultBuilder.Length - 3, 3);
        return new string(resultBuilder.ToString().Reverse().ToArray());
    }
}