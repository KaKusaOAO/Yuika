namespace Yuika.Graphics;

public static class Illegal
{
    public static Exception Value<T>()
    {
        return new IllegalValueException<T>();
    }

    internal class IllegalValueException<T> : VeldridException
    {
    }
}