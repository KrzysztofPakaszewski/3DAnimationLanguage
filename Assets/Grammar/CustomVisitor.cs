namespace anim
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using IToken = Antlr4.Runtime.IToken;

	public class CustomVisitor : AnimationBaseVisitor<object>
	{
		public const string MOVE = "MOVE";

		public const string ROTATE = "ROTATE";

		public const string TRANSFORM = "TRANSFORM";

		Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

		Dictionary<string, Function> functions = new Dictionary<string, Function>();


		Coroutine coroutine = new GameObject().AddComponent(typeof(Coroutine)) as Coroutine;

		private string context = "";

		private string type = "";

		private DateTime startOfLastAction;

		private float lastActionDuration;

		private long timeOfLastAction = 0;

		// in seconds
		private float currentTime = 0f;

		public override object VisitModule(AnimationParser.ModuleContext context) {
			this.startOfLastAction = DateTime.Now;
			return VisitChildren(context); 
		}

		public override object VisitObjectAssign(AnimationParser.ObjectAssignContext context)
		{
			object value = Visit(context.value);
			Variable variable = new Variable(Variable.OBJECT, value);
			variables[context.name.Text] = variable;
			return variable;
		}

		public override object VisitVectorAssign(AnimationParser.VectorAssignContext context) {
			object value = Visit(context.value);
			Variable variable = new Variable(Variable.VECTOR, value);
			variables[context.name.Text] = variable;
			return variable;
		}

		public override object VisitNumberAssign(AnimationParser.NumberAssignContext context) {
			object value = Visit(context.value);
			Variable variable = new Variable(Variable.NUMBER, value);
			variables[context.name.Text]  = variable;
			return variable;
		}

		public override object VisitBooleanAssign(AnimationParser.BooleanAssignContext context) {
			object value = Visit(context.value);
			Variable variable = new Variable(Variable.BOOLEAN, value);
			variables[context.name.Text] =  variable;
			return variable;
		}

		public override object VisitGroupAssign(AnimationParser.GroupAssignContext context) {
			object value = Visit(context.value);
			Variable variable = new Variable(Variable.OBJECT, value);
			variables[context.name.Text] =  variable;
			return variable;
		}

		public override object VisitObjectInitValue(AnimationParser.ObjectInitValueContext context)
		{
			this.type = Variable.OBJECT;
			if (context.SPHERE() != null)
			{
				return GameObject.CreatePrimitive(PrimitiveType.Sphere);
			}
			else if (context.CYLINDER() != null)
			{
				return GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			}
			else if (context.CUBE() != null)
			{
				return GameObject.CreatePrimitive(PrimitiveType.Cube);
			}
			else if (context.PLANE() != null)
			{
				return GameObject.CreatePrimitive(PrimitiveType.Plane);
			}
			else if (context.OBJECT() != null)
			{
				string path = context.OBJECT().GetText();
				path = path.Substring(1, path.Length - 2);
				var result = Resources.Load(path, typeof(GameObject));
				return GameObject.Instantiate(result);
			}
			throw new InvalidObjectValue();
		}

		public override object VisitGroupInitValue(AnimationParser.GroupInitValueContext context) {
			GameObject root = new GameObject();

			this.type = Variable.OBJECT;

			var previous = this.context;
			this.context = Variable.OBJECT;
			foreach( var arg in context._args){
				GameObject go = (GameObject) Visit(arg);
				go.transform.parent = root.transform;
			}

			this.context = previous;
			return root;
		}

		public override object VisitVariableAssign(AnimationParser.VariableAssignContext context) {
			Variable value;
			if (variables.TryGetValue(context.VARIABLE_NAME().GetText(), out value))
			{
				var previous = this.context;

				this.context = value.GetType();

				object newVal = Visit(context.value);

				this.context = previous;

				if (this.type.Equals(value.GetType()))
				{
					Variable variable = new Variable(value.GetType(), newVal);
					variables[context.VARIABLE_NAME().GetText()] = variable;

					return variable;
				}
				else {
					throw new InvalidObjectType();
				}
			}
			else
			{
				throw new VariableUndefined();
			}
		}

		public override object VisitVariable(AnimationParser.VariableContext context) {
			Variable value;
			if (variables.TryGetValue(context.VARIABLE_NAME().GetText(), out value))
			{
				if (this.context.Equals(value.GetType())) {
					this.type = value.GetType();
					return value.GetValue();
				}
				throw new InvalidObjectType();
			}
			else {
				throw new VariableUndefined();
			}
		}

		public override object VisitVectorInitValue( AnimationParser.VectorInitValueContext context) {
			this.type = Variable.VECTOR;
			var previous = this.context;
			this.context = Variable.NUMBER;
			var x = (float) Visit(context.x);
			var y = (float) Visit(context.y);
			var z = (float) Visit(context.z);
			this.context = previous;
			return new Vector3(x, y, z);	
		}

		public override object VisitFactor(AnimationParser.FactorContext context) {
			if (context.expression() != null)
			{
				var result = Visit(context.expression());

				return result;
			}else if (context.variable() != null){

				var result = Visit(context.variable());
				return result;
			}
			else{
				float result = Tools.parseFloat(context.NUMBER().GetText());
				if (context.SUB() != null) {
					result *= -1;
				}
				return result;
			}
		}

		public override object VisitExpression( AnimationParser.ExpressionContext context) {
			this.type = Variable.NUMBER;

			if (context.expression() != null)
			{
				float left = (float)Visit(context.expression());
				float right = (float)Visit(context.term());

				if (context.ADD() != null)
				{
					return left + right;
				}
				else {
					return left - right;
				}
			}
			else {
				var result = Visit(context.term());
				return result;
			}
		}

		public override object VisitTerm(AnimationParser.TermContext context) {
			if (context.term() != null) {
				float left = (float)Visit(context.term());
				float right = (float)Visit(context.factor());

				if (context.MUL() != null)
				{
					return left * right;
				}
				else
				{
					return left / right;
				}

			}else
			{
				var previous = this.context;
				this.context = Variable.NUMBER;

				var result = Visit(context.factor());
				this.context = previous;
				return result;
			}
		}

		public Vector3 getInitialValue(GameObject target, string action) {
			switch (action) {
				case MOVE:
					return target.transform.position;
				case ROTATE:
					return target.transform.rotation.eulerAngles;
				case TRANSFORM:
					return target.transform.localScale;
				default:
					return new Vector3(0, 0, 0);
			}
		}

		public override object VisitWait( AnimationParser.WaitContext context) {
			float value = (float)Visit(context.numberValue());
			this.currentTime += value;
			return value;
		}

		public override object VisitAction(AnimationParser.ActionContext context)
		{
			var previousContext = this.context;

			var action = (string) Visit(context.act);

			this.context = Variable.OBJECT;
			var target =(GameObject) Visit(context.target);

			this.context = Variable.VECTOR;

			Vector3? from = null;
			if (context.prev_val != null)
			{
				from = (Vector3) Visit(context.prev_val);
			}

			Vector3 value = (Vector3) Visit(context.val);

			this.context = Variable.NUMBER;

			if (context.delay != null)
			{
				float delay = (float) Visit(context.delay);
				this.currentTime += delay;
			}
			float time = (float) Visit(context.time);

			this.context = previousContext;

			var diff = (DateTime.Now - this.startOfLastAction).TotalMilliseconds;

			this.currentTime = this.currentTime + (float)diff / 1000;

			coroutine.handleAnimations(action, target, value, time, from, this.currentTime);


			this.currentTime += time;

			this.startOfLastAction = DateTime.Now;
			return null;
		}

		public override object VisitParallel(AnimationParser.ParallelContext context) {
			float maxTime = 0;
			float startTime = this.currentTime;

			foreach(var instruction in context._instr){
				this.currentTime = startTime;
				Visit(instruction);
				if (this.currentTime > maxTime) {
					maxTime = this.currentTime;
				}
			}
			this.currentTime = maxTime;
			return null;
		}

		public override object VisitActionType( AnimationParser.ActionTypeContext context) {
			if (context.MOVE() != null)
			{
				return MOVE;
			}
			else if (context.ROTATE() != null)
			{
				return ROTATE;
			}
			else if (context.TRANSFORM() != null) {
				return TRANSFORM;
			}
			throw new InvalidActionType();
		}

		public override object VisitComparing(AnimationParser.ComparingContext context) {
			var previous = this.context;

			this.context = Variable.NUMBER;
			var left = (float) Visit(context.left);
			var right = (float) Visit(context.right);
			var op = (string) Visit(context.op);

			this.context = previous;

			switch (op) {
				case "<":
					return left < right;
				case "<=":
					return left <= right;
				case ">":
					return left > right;
				case ">=":
					return left >= right;
				case "!=":
					return left != right;
				case "==":
					return left == right;
				default:
					throw new InvalidOperation();
			}
		}

		public override object VisitComparingOperation(AnimationParser.ComparingOperationContext context) {
			if (context.EQUALS() != null)
			{
				return "==";
			}
			else if (context.NOT_EQUALS() != null)
			{
				return "!=";
			}
			else if (context.LT() != null)
			{
				return "<";
			}
			else if (context.LTE() != null)
			{
				return "<=";
			}
			else if (context.GT() != null)
			{
				return ">";
			}
			else if (context.GTE() != null)
			{
				return ">=";
			}else {
				throw new InvalidOperation();
			}
		}

		public override object VisitFactBoolean(AnimationParser.FactBooleanContext context) {
			var previous = this.context;
			this.context = Variable.BOOLEAN;

			if (context.exprBoolean() != null)
			{

				var result = Visit(context.exprBoolean());
				this.context = previous;

				return result;
			}
			else if (context.variable() != null)
			{
				var result = Visit(context.variable());
				this.context = previous;

				return result;
			}
			else
			{
				var result = Visit(context.altBoolean());
				this.context = previous;

				return result;
			}
		}


		public override object VisitExprBoolean(AnimationParser.ExprBooleanContext context) {
			this.type = Variable.BOOLEAN;
			var previous = this.context;
			this.context = Variable.BOOLEAN;

			if (context.exprBoolean() != null)
			{
				bool left = (bool)Visit(context.exprBoolean());
				bool right = (bool)Visit(context.termBoolean());

				this.context = previous;
				return left || right;
			}
			else
			{
				var result = Visit(context.termBoolean());
				this.context = previous;
				return result;
			}
		}

		public override object VisitTermBoolean(AnimationParser.TermBooleanContext context) {
			var previous = this.context;
			this.context = Variable.BOOLEAN;

			if (context.termBoolean() != null)
			{
				bool left = (bool)Visit(context.termBoolean());
				bool right = (bool)Visit(context.factBoolean());

				this.context = previous;

				return left && right;

			}
			else
			{
				var result = Visit(context.factBoolean());
				this.context = previous;
				return result;
			}
		}

		public override object VisitAltBoolean( AnimationParser.AltBooleanContext context) {
			this.type = Variable.BOOLEAN;
			if (context.TRUE() != null)
			{
				return true;
			}
			else if (context.FALSE() != null)
			{
				return false;
			}else if (context.value != null)
			{
				// not value
				var value = (bool)Visit(context.value);
				return !value;
			}
			else
			{
				return Visit(context.comparing());
			}
		}

		public override object VisitWhile(AnimationParser.WhileContext context) {
			var testExpr = (bool)Visit(context.test_expr);
			while (testExpr) {
				Visit(context.body);
				testExpr = (bool)Visit(context.test_expr);
			}
			return testExpr;
		}

		public override object VisitIf(AnimationParser.IfContext context) {
			var test_expr = (bool)Visit(context.if_expr);

			if (test_expr) {
				return Visit(context.body);
			} else if (context.else_expr != null) {
				return Visit(context.else_expr);
			}

			return test_expr;
		}

		public override object VisitElse(AnimationParser.ElseContext context) {
			if (context.sequence() != null)
			{
				return Visit(context.sequence());
			}
			else {
				return Visit(context.@if());
			}
		}

		public override object VisitFunction(AnimationParser.FunctionContext context) {
			Function func = new Function(context.body);

			foreach (var argument in context._args) {
				func.AddVariable((Variable)Visit(argument));
			}
			functions.Add(context.func_name.Text, func);

			return func;
		}

		public override object VisitArgument(AnimationParser.ArgumentContext context) {
			string type = (string) Visit(context.type());
			return new Variable(type, "", context.VARIABLE_NAME().GetText());
		}

		public override object VisitType(AnimationParser.TypeContext context) {
			if (context.OBJECT_TYPE() != null)
			{
				return Variable.OBJECT;
			}
			else if (context.BOOLEAN_TYPE() != null)
			{
				return Variable.BOOLEAN;
			}
			else if (context.NUMBER_TYPE() != null)
			{
				return Variable.NUMBER;
			}
			else if (context.VECTOR_TYPE() != null)
			{
				return Variable.VECTOR;
			}
			else {
				throw new InvalidObjectType();
			}
		}

		public override object VisitCall(AnimationParser.CallContext context)
		{
			Function func;
			if (functions.TryGetValue(context.VARIABLE_NAME().GetText(), out func))
			{
				var previousVariables = this.variables;

				//prepare variables
				prepareArguments(context.arguments()._args, func);

				Visit(func.GetBody());

				this.variables = previousVariables;

				return func;
			}
			else
			{
				throw new FunctionUndefined();
			}
		}

		// arugments when calling a function 
		public void prepareArguments(IList<AnimationParser.ArgValueContext> args, Function function) {
			if (args.Count == function.GetVariables().Count)
			{
				var previous = this.context;

				List<Variable> vars = function.GetVariables();
				for (var i = 0; i < args.Count; i++)
				{
					this.context = vars[i].GetType();
					var value = Visit(args[i]);
					if (this.context.Equals(this.type))
					{
						this.variables[vars[i].GetName()] = new Variable(this.context, value);
					}
					else {
						throw new InvalidObjectType();
					}
				}
				this.context = previous;
			}
			else {
				throw new InvalidArguments();
			}
		}

	}
}

