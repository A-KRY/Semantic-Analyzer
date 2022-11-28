﻿//#undef DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analyzer {

	using Operator = Operand;

	internal class SemanticAnalyzer
	{
		// 唯一实例
		private static SemanticAnalyzer? _uniqueInstance = null;

		public static SemanticAnalyzer Instance
		{
			get
			{
				_uniqueInstance ??= new SemanticAnalyzer();
				return _uniqueInstance;
			}
		}

		// 构造函数
		private SemanticAnalyzer()
		{
			operatorStack = new Stack<Operator>();
			operandStack = new Stack<Operand>();
			postfixExpr = new Queue<Operand>();
		}

		// 输出流
		private StreamWriter? streamWriter = null;

		// 初始化输出流
		public void  InitStreamWriter(String path) {
			if (streamWriter is not null) {
				streamWriter.Close();
			}
			streamWriter = new StreamWriter(path);
			streamWriter.AutoFlush = true;
		}

		//private int quaternionCnt = 0;

		// 临时变量计数
		private int tmpVarCnt = 0;

		// 运算符栈 - 转后缀表达式
		private Stack<Operator> operatorStack;

		public Stack<Operator> OperatorStack
		{
			get => operatorStack;
		}

		// 运算数栈 - 计算后缀表达式
		private Stack<Operand> operandStack;

		public Stack<Operand> OperandStack
		{
			get => operandStack;
		}

		// 后缀表达式
		private Queue<Operand> postfixExpr;

		public Queue<Operand> PostfixExpr
		{
			get => postfixExpr;
		}

		// 生成临时变量名称
		public String NewTemp()
		{
			return "T" + (++tmpVarCnt);
		}


		// 输出四元式
		public void Generate(Operator opr, Operand? opd1, Operand? opd2, Operand result)
		{
			if (streamWriter is null)
			{
				throw new IOException("StreamWriter not Initialized.");
			}

			streamWriter.WriteLine("("+opr+", "+opd1+", "+opd2+", "+result+")");
#if DEBUG
			Console.WriteLine("("+opr+", "+opd1+", "+opd2+", "+result+")");
			Debug.LogWriteLine("("+opr+", "+opd1+", "+opd2+", "+result+")");
#endif
		}

		// 将运算对象送入后缀表达式
		public void Send(Operand opd)
		{
			if (opd.Attribute == Symbol.Operand)
			{
				postfixExpr.Enqueue(opd);
			}
			else if (opd.Attribute == Symbol.RB)
			{
				while (operatorStack.Peek().Attribute != Symbol.LB)
				{
					postfixExpr.Enqueue(operatorStack.Pop());
					CalcNext();
				}
				operatorStack.Pop();
			}
			else
			{
				while (operatorStack.Count != 0 && opd < operatorStack.Peek())
				{
					postfixExpr.Enqueue(operatorStack.Pop());
					CalcNext();
				}
				operatorStack.Push(opd);
			}
#if DEBUG
			Console.Write("oprStack: ");
			Debug.ShowStack(operatorStack);
			
			Debug.LogWrite("oprStack: ");
			Debug.LogShowStack(operatorStack);
#endif
		}

		// 向后处理一个运算符
		private void CalcNext()
		{
			while (postfixExpr.Peek().Attribute == Symbol.Operand)
			{
				operandStack.Push(postfixExpr.Dequeue());
			}

			Operator opr = postfixExpr.Dequeue();
			Operand	opd2 = OperandStack.Pop(), 
				opd1 = OperandStack.Pop(),
				result = new Operand(Symbol.Operand, NewTemp());
			operandStack.Push(result);

			Generate(opr, opd1, opd2, result);
		}

		// 收尾
		public void Finish()
		{
			// 将运算符栈中元素全部送入后缀表达式
			while (operatorStack.Count != 0)
			{
				postfixExpr.Enqueue(operatorStack.Pop());
				CalcNext();
			}


#if DEBUG
			Console.WriteLine();
			Console.WriteLine("Finish.");
			Console.WriteLine();

			Debug.LogWriteLine();
			Debug.LogWriteLine("Finish.");
			Debug.LogWriteLine();
			Debug.Close();
#endif
		}
	}
}
