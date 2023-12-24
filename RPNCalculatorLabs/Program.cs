namespace RPNCalculatorLabs
{
    // Классы в отдельных файлах
    internal class Program
    {
        static void Main()
        {
            while (true)
            {
                Console.Write("Введите математическое выражение (для завершения работы введите q): ");
                string expression = Console.ReadLine(); // При null крашится

                if (expression == "q") break;

                List<Token> parsedExpression = Parse(expression);
                List<Token> rpn = ConvertToRPN(parsedExpression);
                double result = Calculate(rpn);

                Console.WriteLine($"Результат: {result}");
            }
        }
        static int GetPriority(Token operation) // Определяет приоритет операций
        {
            if (operation is Operation op) // Выглядит страшно
            {
                switch (op.symbol)
                {
                    case '+': case '-': return 1;
                    case '*': case '/': return 2;
                    default: return 0;
                }   
            }
            return 0;
        }
        static double GetNumber(object operation, double fisrtOperand, double secondOperand) // Подсчитывает число
        {
            switch (operation)
            {
                case '+': return fisrtOperand + secondOperand;
                case '-': return fisrtOperand - secondOperand;
                case '*': return fisrtOperand * secondOperand;
                case '/': return fisrtOperand / secondOperand;
                default: return 0;
            }
        }
        static List<Token> Parse(string expression) // Парсит введенное выражение
        {
            List<Token> result = new();
            string number = "";

            foreach (char token in expression)
            {
                if (char.IsDigit(token) || token == ',') // Вроде как подружил с дробями
                {
                    number += token;
                }
                else
                {
                    if (number != "")
                    {
                        Number num = new() { value = double.Parse(number) };
                        result.Add(num);
                        number = "";
                    }
                    if (token == '+' || token == '-' || token == '*' || token == '/')
                    {
                        Operation op = new() { symbol = token };
                        result.Add(op);
                    }
                    else if (token == '(' || token == ')')
                    {
                        Parenthesis bracket = new();
                        if (token == '(')
                        {
                            bracket.opening = true;
                        }
                        result.Add(bracket);
                    }
                }
            }

            if (number != "") // Повторяется, +- такое же уже есть в foreach сверху, надо бы исправить как-нибудь
            {
                Number num = new() { value = double.Parse(number) };
                result.Add(num);
            }

            return result;
        }
        static List<Token> ConvertToRPN(List<Token> expression) // Преобразует выражение в Обратную Польскую Нотацию (ОПН)
        {
            Stack<Token> operations = new();
            List<Token> result = new();

            foreach (Token token in expression)
            {
                if (token is Number number)
                {
                    result.Add(number);
                }
                else if (token is Operation operation)
                {
                    while (operations.Count > 0 && GetPriority(operations.Peek()) >= GetPriority(token))
                    {
                        result.Add(operations.Pop());
                    }
                    operations.Push(operation);
                }
                else if (token is Parenthesis bracket)
                {
                    if (bracket.opening)
                    {
                        operations.Push(bracket);
                    }
                    else 
                    {
                        while (operations.Count > 0 && operations.Peek() is not Parenthesis)
                        {
                            result.Add(operations.Pop());
                        }
                        operations.Pop();
                    }
                }
            }

            while (operations.Count > 0)
            {
                result.Add(operations.Pop());
            }

            return result;
        }
        static double Calculate(List<Token> rpn) // Считает итоговое выражение, на этот раз через стэк (спасибо Егор, без тебя бы никто не узнал)
        {
            Stack<double> numbers = new();

            foreach(Token token in rpn)
            {
                if (token is Number number)
                {
                    numbers.Push(number.value);
                }
                else if (token is Operation operation)
                {
                    double secondNum = numbers.Pop();
                    double firstNum = numbers.Pop();
                    char op = operation.symbol;

                    double resultedNum = GetNumber(op, firstNum, secondNum);
                    numbers.Push(resultedNum);
                }
            }

            return numbers.Pop();
        }
    }
}