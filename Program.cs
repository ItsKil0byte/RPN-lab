using System;
using System.Collections.Generic;

namespace RPN
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите математическое выражение: ");
            string expression = Console.ReadLine();
            List<object> parsedExpression = Parse(expression);

            List<object> rpn = ConvertToRPN(parsedExpression);
            Console.WriteLine($"ОПЗ: {string.Join(" ", rpn)}");
            
            float result = Calculate(rpn);
            Console.WriteLine($"Результат: {result}");

            Console.ReadLine();
        }

        static int GetPriority(object operation) // Определяет приоритет операций
        {
            switch (operation)
            {
                case '+': case '-': return 1;
                case '*': case '/': return 2;
                default: return 0;
            }
        }

        static float GetNumber(object operation, float fisrtOperand, float secondOperand) // Подсчитывает число
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

        static List<object> Parse(string expression) // Парсит введенное выражение
        {
            List<object> result = new List<object>();
            string number = "";

            foreach (char token in expression)
            {
                if (token != ' ')
                {
                    if (!char.IsDigit(token))
                    {
                        if (number != "") result.Add(number);
                        result.Add(token);
                        number = "";
                    }
                    else
                    {
                        number += token;
                    }
                }
            }

            if (number != "") result.Add(number);

            return result;
        }

        static List<object> ConvertToRPN(List<object> expression) // Преобразует выражение в Обратную Польскую Нотацию (ОПН)
        {
            Stack<object> operations = new Stack<object>();
            List<object> result = new List<object>();

            foreach (object token in expression)
            {
                if (token is string)
                {
                    result.Add(token);
                }
                else if (token.Equals('+') || token.Equals('-') || token.Equals('*') || token.Equals('/'))
                {
                    while (operations.Count > 0 && GetPriority(operations.Peek()) >= GetPriority(token))
                    {
                        result.Add(operations.Pop());
                    }
                    operations.Push(token);
                }
                else if (token.Equals('('))
                {
                    operations.Push(token);
                }
                else if (token.Equals(')'))
                {
                    while (operations.Count > 0 && !operations.Peek().Equals('('))
                    {
                        result.Add(operations.Pop());
                    }
                    operations.Pop();
                }
            }

            while (operations.Count > 0)
            {
                result.Add(operations.Pop());
            }

            return result;
        }

        static float Calculate(List<object> rpn) // Считает итоговое выражение
        {
            for (int i = 0; i < rpn.Count; i++)
            {
                if (rpn[i] is char)
                {
                    float fisrtNumber = Convert.ToSingle(rpn[i - 2]);
                    float secondNumber = Convert.ToSingle(rpn[i - 1]);

                    float result = GetNumber(rpn[i], fisrtNumber, secondNumber);

                    rpn.RemoveRange(i - 2, 3);
                    rpn.Insert(i - 2, result);
                    i -= 2;
                }
            }

            return (float)rpn[0];
        }
    }
}