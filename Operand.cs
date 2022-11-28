using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace Analyzer {

	using Operator = Operand;
	internal class Operand
	{
		// 符号优先级，行是否小于列
		private static Dictionary<Symbol, Dictionary<Symbol, bool>> priorityTable;

		private void InitPriorityTable()
		{
			priorityTable = new Dictionary<Symbol, Dictionary<Symbol, bool>> {
				[Symbol.PL] = new Dictionary<Symbol, bool> {
					[Symbol.PL] = true,
					[Symbol.MI] = true,
					[Symbol.MU] = true,
					[Symbol.DI] = true,
					[Symbol.LB] = false
				},

				[Symbol.MI] = new Dictionary<Symbol, bool> {
					[Symbol.PL] = true,
					[Symbol.MI] = true,
					[Symbol.MU] = true,
					[Symbol.DI] = true,
					[Symbol.LB] = false
				},

				[Symbol.MU] = new Dictionary<Symbol, bool> {
					[Symbol.PL] = false,
					[Symbol.MI] = false,
					[Symbol.MU] = true,
					[Symbol.DI] = true,
					[Symbol.LB] = false
				},

				[Symbol.DI] = new Dictionary<Symbol, bool> {
					[Symbol.PL] = false,
					[Symbol.MI] = false,
					[Symbol.MU] = true,
					[Symbol.DI] = true,
					[Symbol.LB] = false
				},

				[Symbol.LB] = new Dictionary<Symbol, bool> {
					[Symbol.PL] = false,
					[Symbol.MI] = false,
					[Symbol.MU] = false,
					[Symbol.DI] = false,
					[Symbol.LB] = false
				}
			};
		}

		public Operand()
		{
			name = "";
			InitPriorityTable();
		}

		public Operand(Symbol attribute, String name)
		{
			this.attribute = attribute;
			this.name = name;
			InitPriorityTable();
		}

		// 运算对象属性
		private Symbol attribute;

		public Symbol Attribute
		{
			get => attribute;
			set => attribute = value;
		}

		// 运算对象字符
		private String name;

		public String Name
		{
			get => name;
			set => name = value;
		}

		// 重载 toString() 方法
		public override string ToString()
		{
			return name;
		}

		// 重载比较方法

		public static bool operator> (Operand op1, Operand op2)
		{
			return !priorityTable[op1.attribute][op2.attribute];
		}

		public static bool operator<(Operand op1, Operand op2)
		{
			return priorityTable[op1.attribute][op2.attribute];
		}
	}
}
