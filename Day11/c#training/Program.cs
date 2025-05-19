1) create a program that will take name from user and greet the user

using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.Write("Please enter your name: ");
        string input = Console.ReadLine();

        GreetUser(input);
    }

    static void GreetUser(string nameInput)
    {

        if (string.IsNullOrWhiteSpace(nameInput))
        {
            Console.WriteLine("You didn't enter a valid name. Please try again.");
            return;
        }

        string name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameInput.Trim().ToLower());

        Console.WriteLine($"Hello, {name}! Welcome!");
    }
}


2) Take 2 numbers from user and print the largest


using System;

class Program
{
    static void Main()
    {
        double number1 = GetValidNumber("enter the first number: ");
        double number2 = GetValidNumber("enter the second number: ");

        PrintLargest(number1, number2);
    }

    static double GetValidNumber(string prompt)
    {
        double number;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("input cannot be empty. please try again.");
                continue;
            }

            if (!double.TryParse(input.Trim(), out number))
            {
                Console.WriteLine("invalid number. please enter a valid numeric value.");
                continue;
            }

            break;
        }

        return number;
    }


    static void PrintLargest(double num1, double num2)
    {
        if (num1 > num2)
        {
            Console.WriteLine($"larger number is: {num1}");
        }
        else if (num2 > num1)
        {
            Console.WriteLine($"larger number is: {num2}");
        }
        else
        {
            Console.WriteLine("both numbers are equal.");
        }
    }
}


3) Take 2 numbers from user, check the operation user wants to perform (+,-,*,/). Do the operation and print the result


using System;

class Program
{
    static void Main()
    {
        double num1 = GetValidNumber("enter the first number: ");
        char operation = GetValidOperation();
        double num2 = GetValidNumber("enter the second number: ");


        double? result = PerformOperation(num1, num2, operation);

        if (result.HasValue)
        {
            Console.WriteLine($"result: {num1} {operation} {num2} = {result.Value}");
        }
    }


    static double GetValidNumber(string prompt)
    {
        double number;

        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("input cannot be empty. please try again.");
                continue;
            }

            if (!double.TryParse(input.Trim(), out number))
            {
                Console.WriteLine("invalid number. please enter a valid numeric value.");
                continue;
            }

            return number;
        }
    }

    static char GetValidOperation()
    {
        while (true)
        {
            Console.Write("enter operation (+, -, *, /): ");
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input.Trim().Length != 1)
            {
                Console.WriteLine("invalid input. please enter a single character: +, -, *, or /");
                continue;
            }

            char op = input.Trim()[0];

            if (op == '+' || op == '-' || op == '*' || op == '/')
            {
                return op;
            }

            Console.WriteLine("invalid operation. please enter one of +, -, *, /");
        }
    }

    static double? PerformOperation(double a, double b, char op)
    {
        switch (op)
        {
            case '+':
                return a + b;

            case '-':
                return a - b;

            case '*':
                return a * b;

            case '/':
                if (b == 0)
                {
                    Console.WriteLine(" division by zero is not allowed.");
                    return null;
                }
                return a / b;

            default:
                Console.WriteLine("unknown operation.");
                return null;
        }
    }
}


4) Take username and password from user. Check if user name is "Admin" and password is "pass" if yes then print success message. Give 3 attempts to user. In the end of eh 3rd attempt if user still is unable to provide valid creds then exit the application after print the message "Invalid attempts for 3 times. Exiting...."


using System;

class Program
{
    static void Main()
    {
        const string validUsername = "Admin";
        const string validPassword = "pass";
        const int maxAttempts = 3;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            if (username == validUsername && password == validPassword)
            {
                Console.WriteLine("login successful. Welcome Admin!");
                return;
            }
            else
            {
                Console.WriteLine($"invalid credentials. Attempt {attempt} of {maxAttempts}.");

                if (attempt == maxAttempts)
                {
                    Console.WriteLine("invalid attempts for 3 times. Exiting....");
                }
            }
        }
    }
}

5) Take 10 numbers from user and print the number of numbers that are divisible by 7

using System;

class Program
{
    static void Main()
    {
        int totalNumbers = 10;
        int divisibleBySevenCount = 0;

        for (int i = 1; i <= totalNumbers; i++)
        {
            int number = GetValidInteger($"enter number {i}: ");

            if (number % 7 == 0)
            {
                divisibleBySevenCount++;
            }
        }

        Console.WriteLine($"Count of numbers divisible by 7: {divisibleBySevenCount}");
    }


    static int GetValidInteger(string prompt)
    {
        int number;
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("input cannot be empty. please enter a valid integer.");
                continue;
            }

            if (!int.TryParse(input.Trim(), out number))
            {
                Console.WriteLine("invalid input. please enter a valid integer.");

                continue;
            }

            return number;
        }
    }
}




6) Count the Frequency of Each Element
Given an array, count the frequency of each element and print the result.
Input: {1, 2, 2, 3, 4, 4, 4}

using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("Please enter the numbers separated by space:");
        int[] numbers = GetValidIntegerArrayFromUser();

        Dictionary<int, int> frequencyMap = new Dictionary<int, int>();

        foreach (int num in numbers)
        {
            if (frequencyMap.ContainsKey(num))
            {
                frequencyMap[num]++;
            }
            else
            {
                frequencyMap[num] = 1;
            }
        }

        Console.WriteLine("\nFrequency of each number:");
        foreach (var pair in frequencyMap)
        {
            Console.WriteLine($"{pair.Key} -> {pair.Value} times");
        }
    }


    static int[] GetValidIntegerArrayFromUser()
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Please enter at least one number:");
                continue;
            }

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<int> numbers = new List<int>();

            bool allValid = true;

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int number))
                {
                    numbers.Add(number);
                }
                else
                {
                    Console.WriteLine($"Invalid number detected: '{part}'. Please enter only integers.");
                    allValid = false;
                    break;
                }
            }

            if (allValid && numbers.Count > 0)
            {
                return numbers.ToArray();
            }

            Console.WriteLine("Please try again. Enter numbers separated by spaces:");
        }
    }
}


output
1 occurs 1 times  
2 occurs 2 times  
3 occurs 1 times  
4 occurs 3 times

7) create a program to rotate the array to the left by one position.
Input: {10, 20, 30, 40, 50}
Output: {20, 30, 40, 50, 10}

using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("Please enter array elements separated by space:");
        int[] array = GetValidIntegerArrayFromUser();

        if (array.Length < 2)
        {
            Console.WriteLine("Hey User!!, Array must have at least 2 elements to rotate.");
            return;
        }

        RotateLeftByOne(array);

        Console.WriteLine("\nHey User!!, Array after rotating left by one position:");
        Console.WriteLine(string.Join(" ", array));
    }


    static void RotateLeftByOne(int[] arr)
    {
        int first = arr[0];
        for (int i = 0; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }
        arr[arr.Length - 1] = first;
    }

    static int[] GetValidIntegerArrayFromUser()
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Hey User!!, Input cannot be empty. Please enter at least 2 numbers:");
                continue;
            }

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<int> numbers = new List<int>();

            bool allValid = true;

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int number))
                {
                    numbers.Add(number);
                }
                else
                {
                    Console.WriteLine($"Hey User!!, Invalid number: '{part}'. Please enter only integers.");
                    allValid = false;
                    break;
                }
            }

            if (allValid && numbers.Count >= 2)
            {
                return numbers.ToArray();
            }

            Console.WriteLine("Please enter at least 2 valid integers:");
        }
    }
}



8) Given two integer arrays, merge them into a single array.
Input: {1, 3, 5} and {2, 4, 6}
Output: {1, 3, 5, 2, 4, 6}

using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter elements of the first array (separated by spaces):");
        int[] array1 = GetValidIntegerArrayFromUser();

        Console.WriteLine("Enter elements of the second array (separated by spaces):");
        int[] array2 = GetValidIntegerArrayFromUser();

        int[] mergedArray = MergeArrays(array1, array2);

        Console.WriteLine("\nMerged array:");
        Console.WriteLine(string.Join(" ", mergedArray));
    }


    static int[] MergeArrays(int[] arr1, int[] arr2)
    {
        int[] merged = new int[arr1.Length + arr2.Length];
        arr1.CopyTo(merged, 0);              
        arr2.CopyTo(merged, arr1.Length);     
        return merged;
    }


    static int[] GetValidIntegerArrayFromUser()
    {
        while (true)
        {
            string input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Please enter at least one number:");
                continue;
            }

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<int> numbers = new List<int>();
            bool allValid = true;

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int number))
                {
                    numbers.Add(number);
                }
                else
                {
                    Console.WriteLine($"Invalid input '{part}'. Please enter only integers.");
                    allValid = false;
                    break;
                }
            }

            if (allValid && numbers.Count > 0)
            {
                return numbers.ToArray();
            }

            Console.WriteLine("Please re-enter valid space-separated integers:");
        }
    }
}


9) Write a program that:

Has a predefined secret word (e.g., "GAME").

Accepts user input as a 4-letter word guess.

Compares the guess to the secret word and outputs:

X Bulls: number of letters in the correct position.

Y Cows: number of correct letters in the wrong position.

Continues until the user gets 4 Bulls (i.e., correct guess).

Displays the number of attempts.

Bull = Correct letter in correct position.

Cow = Correct letter in wrong position.

Secret Word	User Guess	Output	Explanation
GAME	GAME	4 Bulls, 0 Cows	Exact match
GAME	MAGE	1 Bull, 3 Cows	A in correct position, MGE misplaced
GAME	GUYS	1 Bull, 0 Cows	G in correct place, rest wrong
GAME	AMGE	2 Bulls, 2 Cows	A, E right; M, G misplaced
NOTE	TONE	2 Bulls, 2 Cows	O, E right; T, N misplaced





using System;

class Program
{
    static void Main()
    {
        const string secretWord = "GAME";
        int attempts = 0;

        Console.WriteLine("Welcome to Bulls and Cows!");
        Console.WriteLine("Try to guess the 4-letter secret word.");
        Console.WriteLine("Type only letters (A-Z). Case doesn't matter.\n");

        while (true)
        {
            string guess = GetValidGuess();
            attempts++;

            int bulls = 0, cows = 0;
            bool[] secretUsed = new bool[4];
            bool[] guessUsed = new bool[4];


            for (int i = 0; i < 4; i++)
            {
                if (guess[i] == secretWord[i])
                {
                    bulls++;
                    secretUsed[i] = true;
                    guessUsed[i] = true;
                }
            }


            for (int i = 0; i < 4; i++)
            {
                if (guessUsed[i]) continue;

                for (int j = 0; j < 4; j++)
                {
                    if (!secretUsed[j] && guess[i] == secretWord[j])
                    {
                        cows++;
                        secretUsed[j] = true;
                        break;
                    }
                }
            }

            Console.WriteLine($"Result: {bulls} Bulls, {cows} Cows\n");

            if (bulls == 4)
            {
                Console.WriteLine($"Correct! You've guessed the word '{secretWord}' in {attempts} attempts.");
                break;
            }
        }
    }

    static string GetValidGuess()
    {
        while (true)
        {
            Console.Write("Enter your 4-letter guess: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Try again.\n");
                continue;
            }

            string guess = input.Trim().ToUpper();

            if (guess.Length != 4)
            {
                Console.WriteLine("Please enter exactly 4 letters.\n");
                continue;
            }

            if (!IsAllLetters(guess))
            {
                Console.WriteLine("Input must contain only alphabetic characters (A-Z). Try again.\n");
                continue;
            }

            return guess;
        }
    }


    static bool IsAllLetters(string word)
    {
        foreach (char c in word)
        {
            if (!char.IsLetter(c))
                return false;
        }
        return true;
    }
}


10) write a program that accepts a 9-element array representing a Sudoku row.

Validates if the row:

Has all numbers from 1 to 9.

Has no duplicates.

Displays if the row is valid or invalid.


using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter 9 numbers (1-9) for a Sudoku row:");

        int[] sudokuRow = GetValidSudokuRow();

        if (IsValidSudokuRow(sudokuRow))
        {
            Console.WriteLine("The Sudoku row is valid.");
        }
        else
        {
            Console.WriteLine("The Sudoku row is invalid.");
        }
    }


    static int[] GetValidSudokuRow()
    {
        while (true)
        {
            Console.Write("Enter 9 numbers (1-9) separated by spaces: ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty. Try again.\n");
                continue;
            }

            string[] parts = input.Trim().Split();

            if (parts.Length != 9)
            {
                Console.WriteLine("You must enter exactly 9 numbers. Try again.\n");
                continue;
            }

            int[] numbers = new int[9];
            bool valid = true;

            for (int i = 0; i < 9; i++)
            {
                if (!int.TryParse(parts[i], out numbers[i]) || numbers[i] < 1 || numbers[i] > 9)
                {
                    Console.WriteLine($"'{parts[i]}' is not a valid number between 1 and 9.\n");
                    valid = false;
                    break;
                }
            }

            if (valid)
                return numbers;
        }
    }


    static bool IsValidSudokuRow(int[] row)
    {
        return row.Distinct().Count() == 9;
    }
}


11) 11) In the question ten extend it to validate a sudoku game. 
Validate all 9 rows (use int[,] board = new int[9,9])


using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter the Sudoku board (9 rows of 9 numbers from 1 to 9):");
        int[,] board = GetSudokuBoard();

        if (IsValidSudokuBoard(board))
        {
            Console.WriteLine("The Sudoku board is valid.");
        }
        else
        {
            Console.WriteLine("The Sudoku board is invalid.");
        }
    }


    static int[,] GetSudokuBoard()
    {
        int[,] board = new int[9, 9];

        for (int row = 0; row < 9; row++)
        {
            while (true)
            {
                Console.Write($"Enter row {row + 1} (9 numbers 1–9 separated by spaces): ");
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty.\n");
                    continue;
                }

                string[] parts = input.Trim().Split();
                if (parts.Length != 9)
                {
                    Console.WriteLine("You must enter exactly 9 numbers.\n");
                    continue;
                }

                bool valid = true;
                for (int col = 0; col < 9; col++)
                {
                    if (!int.TryParse(parts[col], out int num) || num < 1 || num > 9)
                    {
                        Console.WriteLine($"'{parts[col]}' is not a valid number (1-9).\n");
                        valid = false;
                        break;
                    }
                    board[row, col] = num;
                }

                if (valid) break;
            }
        }

        return board;
    }


    static bool IsValidSudokuBoard(int[,] board)
    {

        for (int row = 0; row < 9; row++)
        {
            if (!IsUnique(GetRow(board, row))) return false;
        }


        for (int col = 0; col < 9; col++)
        {
            if (!IsUnique(GetColumn(board, col))) return false;
        }


        for (int startRow = 0; startRow < 9; startRow += 3)
        {
            for (int startCol = 0; startCol < 9; startCol += 3)
            {
                if (!IsUnique(GetSubGrid(board, startRow, startCol))) return false;
            }
        }

        return true;
    }


    static int[] GetRow(int[,] board, int row)
    {
        int[] result = new int[9];
        for (int i = 0; i < 9; i++)
            result[i] = board[row, i];
        return result;
    }


    static int[] GetColumn(int[,] board, int col)
    {
        int[] result = new int[9];
        for (int i = 0; i < 9; i++)
            result[i] = board[i, col];
        return result;
    }


    static int[] GetSubGrid(int[,] board, int startRow, int startCol)
    {
        int[] result = new int[9];
        int index = 0;

        for (int i = startRow; i < startRow + 3; i++)
        {
            for (int j = startCol; j < startCol + 3; j++)
            {
                result[index++] = board[i, j];
            }
        }

        return result;
    }


    static bool IsUnique(int[] numbers)
    {
        return numbers.Distinct().Count() == 9;
    }
}


12) Write a program that:

Takes a message string as input (only lowercase letters, no spaces or symbols).

Encrypts it by shifting each character forward by 3 places in the alphabet.

Decrypts it back to the original message by shifting backward by 3.

Handles wrap-around, e.g., 'z' becomes 'c'.

Examples
Input:     hello
Encrypted: khoor
Decrypted: hello
-------------
Input:     xyz
Encrypted: abc
Test cases
| Input | Shift | Encrypted | Decrypted |
| ----- | ----- | --------- | --------- |
| hello | 3     | khoor     | hello     |
| world | 3     | zruog     | world     |
| xyz   | 3     | abc       | xyz       |
| apple | 1     | bqqmf     | apple     |


using System;

class Program
{
    static void Main()
    {
        Console.Write("Enter a message (only lowercase letters, no spaces or symbols): ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) || !IsValidInput(input))
        {
            Console.WriteLine("Invalid input. Please use only lowercase letters a–z with no spaces or symbols.");
            return;
        }

        Console.Write("Enter the shift value (e.g., 3): ");
        if (!int.TryParse(Console.ReadLine(), out int shift))
        {
            Console.WriteLine("Invalid shift. Please enter a valid number.");
            return;
        }

        string encrypted = Encrypt(input, shift);
        string decrypted = Decrypt(encrypted, shift);

        Console.WriteLine($"\nOriginal Message: {input}");
        Console.WriteLine($"Encrypted: {encrypted}");
        Console.WriteLine($"Decrypted: {decrypted}");
    }


    static string Encrypt(string message, int shift)
    {
        char[] encrypted = new char[message.Length];

        for (int i = 0; i < message.Length; i++)
        {
            char c = message[i];
            encrypted[i] = (char)((((c - 'a') + shift) % 26) + 'a');
        }

        return new string(encrypted);
    }


    static string Decrypt(string encrypted, int shift)
    {
        char[] decrypted = new char[encrypted.Length];

        for (int i = 0; i < encrypted.Length; i++)
        {
            char c = encrypted[i];
            decrypted[i] = (char)((((c - 'a') - shift + 26) % 26) + 'a');
        }

        return new string(decrypted);
    }

 
    static bool IsValidInput(string input)
    {
        foreach (char c in input)
        {
            if (c < 'a' || c > 'z')
                return false;
        }
        return true;
    }
}
