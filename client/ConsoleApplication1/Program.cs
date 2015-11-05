using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

class Solution
{
    static void insertionSort(int[] ar)
    {
        int v = ar[ar.Length - 1];
        int i = 0;
        ar[ar.Length - 1] = ar] = ar[ar.Length - 2];
        for (i = ar.Length - 2; i >= 0; i--)
        {
            if(i != 0 && ar[i] > v)
            {
                ar[i+1] = ar[i];
                PrintArray(ar);
            }
            else
            {
                ar[i+1] = v;
                PrintArray(ar);
                break;
            }
        }
    }

    static void PrintArray(int[] ar)
    {
        for (int i = 0; i < ar.Length; i++)
        {
            Console.Write("{0} ", ar[i]);
        }
        Console.WriteLine("");
    }
    /* Tail starts here */
    static void Main(String[] args)
    {

        int _ar_size;
        _ar_size = Convert.ToInt32(Console.ReadLine());
        int[] _ar = new int[_ar_size];
        String elements = Console.ReadLine();
        String[] split_elements = elements.Split(' ');
        for (int _ar_i = 0; _ar_i < _ar_size; _ar_i++)
        {
            _ar[_ar_i] = Convert.ToInt32(split_elements[_ar_i]);
        }

        insertionSort(_ar);
        Console.ReadKey();
    }
}
