using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Web;
using Moq;

namespace MVCWhenThenFramework
{
    public static class Extensions
    {
        public static void SetupUserHostAddress(this HttpRequestBase request, string address)
        {
            if(address == null) throw new ArgumentNullException("address");

            var mock = Mock.Get(request);

            mock.Setup(x => x.UserHostAddress).Returns(address);

        }

        /// <summary>
        /// need to invoke a base class method that is over ridden by the subclass?
        /// no fear use this extension shamelessly taken from http://www.simplygoodcode.com/2012/08/invoke-base-method-using-reflection.html
        /// 
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="targetObject"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static object InvokeNotOverride(this MethodInfo methodInfo, object targetObject, params object[] arguments)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length == 0)
            {
                if (arguments != null && arguments.Length != 0)
                    throw new Exception("Arguments cont doesn't match");
            }
            else
            {
                if (parameters.Length != arguments.Length)
                    throw new Exception("Arguments cont doesn't match");
            }

            Type returnType = null;
            if (methodInfo.ReturnType != typeof(void))
            {
                returnType = methodInfo.ReturnType;
            }

            var type = targetObject.GetType();
            var dynamicMethod = new DynamicMethod("", returnType,
                    new Type[] { type, typeof(Object) }, type);

            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0); // this

            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];

                iLGenerator.Emit(OpCodes.Ldarg_1); // load array argument

                // get element at index
                iLGenerator.Emit(OpCodes.Ldc_I4_S, i); // specify index
                iLGenerator.Emit(OpCodes.Ldelem_Ref); // get element

                var parameterType = parameter.ParameterType;
                if (parameterType.IsPrimitive)
                {
                    iLGenerator.Emit(OpCodes.Unbox_Any, parameterType);
                }
                else if (parameterType == typeof(object))
                {
                    // do nothing
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Castclass, parameterType);
                }
            }

            iLGenerator.Emit(OpCodes.Call, methodInfo);
            iLGenerator.Emit(OpCodes.Ret);

            return dynamicMethod.Invoke(null, new object[] { targetObject, arguments });
        }

    }
}
