namespace Calculator;
using System;
using System.Linq.Expressions;

public partial class NewMain : ContentPage
{
	public NewMain()
	{
		InitializeComponent();
        OnClear(this, null);
    }
    string currentEntry = "";
    int currentState = 1;
    string mathOperator;
    double firstNumber, secondNumber;
    string decimalFormat = "N0";
    bool expression = false;

    Stack<double> values = new Stack<double>();
    Stack<Char> ops = new Stack<char>();



    void OnSelectNumber(object sender, EventArgs e)
    {
       

        Button button = (Button)sender;
        string pressed = button.Text;

        currentEntry += pressed;

        if ((this.resultText.Text == "0" && pressed == "0")
            || (currentEntry.Length <= 1 && pressed != "0")
            || currentState < 0)
        {
            this.resultText.Text = "";
            if (currentState < 0)
                currentState *= -1;
        }

        if (pressed == "." && decimalFormat != "N2")
        {
            decimalFormat = "N2";
        }

        this.resultText.Text += pressed;

        //If current pressed is a number push it the stack

        this.values.Push(int.Parse(pressed));
    }

    void OnSelectOperator(object sender, EventArgs e)
    {
        
        LockNumberValue(resultText.Text);

        currentState = -2;
        Button button = (Button)sender;
        string pressed = button.Text;
        mathOperator = pressed;

       // if the pressed is a operator then push it onn the stack

        if (pressed == "(")
        {
            this.ops.Push(char.Parse(pressed));
            expression = true;
        }
        else if (pressed == ")")
        {
            while (this.ops.Peek() != '(')
            {
                this.values.Push(Calculator.Calculate(this.values.Pop(), this.values.Pop(), (this.ops.Pop()).ToString()));
            }
            this.ops.Pop();
        }
        // If pressed is an operator

        else if (pressed == "+" || pressed == "-" || pressed == "×"  || pressed == "/")
        {
            // While top of 'ops' has same
            // or greater precedence to current
            // token, which is an operator.
            // Apply operator on top of 'ops'
            // to top two elements in values stack

            while (this.ops.Count > 0 && hasPrecedence(char.Parse(pressed), this.ops.Peek()))
            {
                this.values.Push(Calculator.Calculate(this.values.Pop(), this.values.Pop(), (this.ops.Pop()).ToString()));
            }
            // push current pressed to ops

            this.ops.Push(char.Parse(pressed));
        }

        // Entire expression has been
        // parsed at this point, apply remaining
        // ops to remaining values
        //while (this.ops.Count > 0)
        //{
        //    this.values.Push(Calculator.Calculate(this.values.Pop(), this.values.Pop(), (this.ops.Pop()).ToString()));
        //}

        // Top of 'values' contains
        // result, return it

    }

    // Returns true if 'op2' has
    // higher or same precedence as 'op1',
    // otherwise returns false.
    public static bool hasPrecedence(char op1,
                                    char op2)
    {
        if (op2 == '(' || op2 == ')')
        {
            return false;
        }
        if ((op1 == '*' || op1 == '/') &&
            (op2 == '+' || op2 == '-'))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //public static void applyOp(char op, int b, int a)
    //{
    //    switch(op)
    //    {
    //        case '+':
    //            return 
    //    }
    //}

    private void LockNumberValue(string text)
    {
        double number;
        if (double.TryParse(text, out number))
        {
            if (currentState == 1)
            {
                firstNumber = number;
            }
            else
            {
                secondNumber = number;
            }

            currentEntry = string.Empty;
        }
    }

    void OnClear(object sender, EventArgs e)
    {
        firstNumber = 0;
        secondNumber = 0;
        currentState = 1;
        decimalFormat = "N0";
        this.resultText.Text = "0";
        currentEntry = string.Empty;
    }

    void OnCalculate(object sender, EventArgs e)
    {
        if (currentState == 2)
        {
            if (secondNumber == 0)
                LockNumberValue(resultText.Text);

            double result = Calculator.Calculate(firstNumber, secondNumber, mathOperator);

            this.CurrentCalculation.Text = $"{firstNumber} {mathOperator} {secondNumber}";

            this.resultText.Text = result.ToTrimmedString(decimalFormat);
            firstNumber = result;
            secondNumber = 0;
            currentState = -1;
            currentEntry = string.Empty;
        }
        else if (expression)
        {
            this.resultText.Text = this.values.Pop().ToString();
        }
        
    }

    void OnNegative(object sender, EventArgs e)
    {
        if (currentState == 1)
        {
            secondNumber = -1;
            mathOperator = "×";
            currentState = 2;
            OnCalculate(this, null);
        }
    }

    void OnPercentage(object sender, EventArgs e)
    {
        if (currentState == 1)
        {
            LockNumberValue(resultText.Text);
            decimalFormat = "N2";
            secondNumber = 0.01;
            mathOperator = "×";
            currentState = 2;
            OnCalculate(this, null);
        }
    }

    void OnSqrt(object sender, EventArgs e)
    {
        if(currentState == 1)
        {
            LockNumberValue(resultText.Text);
            mathOperator = "sqrt";
            currentState = 2;

        }
    }

    void OnModulo(object sender, EventArgs e)
    {
        if(currentState == 1)
        {
            LockNumberValue(resultText.Text);
            mathOperator = "%";
            currentState = 2;
        }
    }
}

