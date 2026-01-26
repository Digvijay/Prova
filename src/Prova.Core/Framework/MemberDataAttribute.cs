namespace Prova
{
    /// <summary>
    /// Attribute that provides data for a <see cref="TheoryAttribute"/> from a member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class MemberDataAttribute : Attribute
    {
        /// <summary>Gets the name of the member that provides the data.</summary>
        public string MemberName { get; }
        
        /// <summary>Gets the parameters passed to the member.</summary>
        public object[] Parameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberDataAttribute"/> class.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="parameters">The parameters for the member.</param>
        public MemberDataAttribute(string memberName, params object[] parameters)
        {
            MemberName = memberName;
            Parameters = parameters;
        }
    }
}
