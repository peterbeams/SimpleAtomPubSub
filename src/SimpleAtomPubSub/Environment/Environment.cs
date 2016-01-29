namespace SimpleAtomPubSub.Environment
{
    public class Environment
    {
        public static IEnvironment Current { get; internal set; } = new EnvironmentImpl();
    }
}