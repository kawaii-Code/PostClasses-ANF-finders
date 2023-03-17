using System.Text;

namespace PostClasses;

public static class PostClasses
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
        int m = int.Parse(Input("M >> "));
        switch (m)
        {
            case <= 0:
                throw new ArgumentException($"Variable count must be greater than 0! Was {m}.");
            case >= 31:
                throw new ArgumentException($"Variable count is too big! The program will be slow for {m} variables.");
        }

        string eval = Input("Eval >> ");
        if (eval.Length != 1 << m)
            throw new ArgumentException($"Eval length should correspond with the number of variables! Should have been {1 << m}, was {eval.Length}.");

        return ParseEval(eval).ToArray();
    }

    private static void Intro()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("== Post class finder ==");
        Console.WriteLine("This program will find all the Post classes the function belongs to by the eval of that function.");
        Console.WriteLine("Specify the number of variables in your function (M) and it's eval (e.g. 10011010):");
        Console.WriteLine();
        Console.ResetColor();
    }

    private static string Column(string columnName, int spaceCount = 3, bool isLast = false)
    {
        StringBuilder builder = new();
        builder.Append('|');
        builder.Append(' ', spaceCount);
        builder.Append(columnName.PadRight(spaceCount + 1));
        if (isLast)
            builder.Append("|\n");
        
        return builder.ToString();
    }
    
    private static string ToPlus(bool val) => val ? "+" : "-";
    
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
        Console.WriteLine(" _______________________________________ ");
        Console.Write(Column("P0"));
        Console.Write(Column("P1"));
        Console.Write(Column("L"));
        Console.Write(Column("S"));
        Console.Write(Column("M", isLast: true));
        Console.Write(Column(ToPlus(BelongsToP0(eval))));
        Console.Write(Column(ToPlus(BelongsToP1(eval))));
        Console.Write(Column(ToPlus(BelongsToL(eval))));
        Console.Write(Column(ToPlus(BelongsToS(eval))));
        Console.Write(Column(ToPlus(BelongsToM(eval)), isLast: true));
        Console.WriteLine(" --------------------------------------- ");
        Console.ResetColor();

        goto Start;
    }
    
    private static bool BelongsToP0(bool[] eval) => !eval[0];
    
    private static bool BelongsToP1(bool[] eval) => eval[^1];
    
    private static bool BelongsToL(bool[] eval)
    {
        for (int i = 0; i < eval.Length; i++)
        {
            if ((i & i - 1) == 0)
                continue;

            bool ifWasLinear = eval[0];
            for (int j = 1 << (int)Math.Log2(i); j > 0; j >>= 1)
                if ((j & i) != 0)
                    ifWasLinear ^= eval[j] ^ eval[0];

            if (ifWasLinear != eval[i])
                return false;
        }
        
        return true;
    }
    
    private static bool BelongsToS(bool[] eval)
    {
        for (int i = 0; i < eval.Length / 2; i++)
            if (eval[i] == eval[^(i + 1)])
                return false;
        return true;
    }

    private static bool BelongsToM(bool[] eval)
    {
        if (eval[0])
            return eval.All(e => e);
        
        for (int i = 0; i < eval.Length; i++)
        {
            if (!eval[i])
                continue;

            for (int j = 1; j < eval.Length; j <<= 1)
                if (!eval[i | j])
                    return false;
        }
        
        return true;
    }
}