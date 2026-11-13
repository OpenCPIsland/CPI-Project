// See MathosLicense.txt

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using Mathos.Parser;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory(ActionCategory.Math)]
    [Tooltip("Math expression action. Enter the expression using variable names and common math syntax. Uses Mathos parser.")]
    public class MathExpression : FsmStateAction
    {
        [UIHint(UIHint.TextArea)]
        [Tooltip("Expression to evaluate. Accepts float, int, and bool variable names. Also: Time.deltaTime, ")]
        public FsmString expression;

        [Tooltip("Store the result in a float variable")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeResultAsFloat;

        [Tooltip("Store the result in an int variable")]
        [UIHint(UIHint.Variable)]
        public FsmInt storeResultAsInt;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;
        
        private MathParser parser;
        private string cachedExpression;
        private ReadOnlyCollection<string> tokens;
        private readonly List<NamedVariable> usedVariables = new List<NamedVariable>();

        public class Property
        {
            public string path;

        }

        public override void Awake()
        {
            parser = new MathParser();

            parser.LocalVariables["Time.deltaTime"] = 0;
        }

        public override void OnEnter()
        {
            DoMathExpression();

            if (!everyFrame)
                Finish();
        }

        public override void OnUpdate()
        {
            parser.LocalVariables["Time.deltaTime"] = Time.deltaTime;

            DoMathExpression();
        }

        private void DoMathExpression()
        {
            var result = ParseExpression();

            if (!storeResultAsFloat.IsNone)
            {
                storeResultAsFloat.Value = (float) result;
            }

            if (!storeResultAsInt.IsNone)
            {
                storeResultAsInt.Value = Mathf.FloorToInt((float) result);
            }
        }

        public double ParseExpression()
        {
            if (expression.Value != cachedExpression)
            {
                BuildAndCacheExpression();
            }

            for (var i = 0; i < usedVariables.Count; i++)
            {
                var variable = usedVariables[i];
                switch (variable.VariableType)
                {
                    case VariableType.Float:
                        parser.LocalVariables[variable.Name] = ((FsmFloat) variable).Value;
                        break;

                    case VariableType.Int:
                        parser.LocalVariables[variable.Name] = ((FsmInt) variable).Value;
                        break;

                    case VariableType.Bool:
                        parser.LocalVariables[variable.Name] = ((FsmBool) variable).Value ? 1 : 0;
                        break;

                    default:
                        Debug.Log("Unsupported variable type: " + variable.Name + " (" + variable.VariableType + ")");
                        break;
                }

                // parser.LocalVariables.Add(variable.Name, (double) variable.RawValue);
            }

            return parser.Parse(tokens);
        }

        private void BuildAndCacheExpression()
        {
            if (parser == null) parser = new MathParser();

            // we can parse tokens once and save them

            tokens = parser.GetTokens(expression.Value);

            // remove any variables previously added to parser.LocalVariables
            // this should be cheaper than getting a new parser

            foreach (var variable in usedVariables)
            {
                parser.LocalVariables.Remove(variable.Name);
            }

            // find variables in tokens and add them to used variables list

            usedVariables.Clear();

            foreach (var token in tokens)
            {
                var variable = Fsm.Variables.FindVariable(token) ?? FsmVariables.GlobalVariables.FindVariable(token);
                if (variable != null && !usedVariables.Contains(variable))
                {
                    usedVariables.Add(variable);
                }
            }

            // store the expression so we know if its changed

            cachedExpression = expression.Value;
        }

        #if UNITY_EDITOR

        public override string AutoName()
        {
            var name = " = " + expression.Value;
            if (!storeResultAsFloat.IsNone)
                name = storeResultAsFloat.Name + name;
            else if (!storeResultAsInt.IsNone)
                name = storeResultAsInt.Name + name;
            return name;
        }

        public override string ErrorCheck()
        {
            if (FsmString.IsNullOrEmpty(expression)) return "";

            try
            {
                ParseExpression();
                return "";
            }
            catch (Exception e)
            {
                if (e is ArithmeticException)
                    return "@expression:" + e.Message;
                return "@expression: Parsing error...";
            }
        }


        #endif

    }

}
