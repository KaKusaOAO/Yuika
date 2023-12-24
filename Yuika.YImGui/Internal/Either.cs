namespace Yuika.YImGui.Internal;

internal abstract class Either<TLeft, TRight>
{
    public abstract TLeft Left { get; }
    public abstract TRight Right { get; }
    public abstract bool IsLeft { get; }
    public abstract bool IsRight { get; }
    
    public TLeft LeftOr(TLeft other) => IsLeft ? Left : other;
    public TRight RightOr(TRight other) => IsRight ? Right : other;

    public static Either<TLeft, TRight> OfLeft(TLeft value) => new LeftResult(value);
    public static Either<TLeft, TRight> OfRight(TRight value) => new RightResult(value);

    private class LeftResult : Either<TLeft, TRight>
    {
        private readonly TLeft _value;
        
        public LeftResult(TLeft value) => _value = value;
        
        public override TLeft Left => _value;
        public override TRight Right => throw new Exception();
        public override bool IsLeft => true;
        public override bool IsRight => false;
    }
    
    private class RightResult : Either<TLeft, TRight>
    {
        private readonly TRight _value;
        
        public RightResult(TRight value) => _value = value;
        
        public override TLeft Left => throw new Exception();
        public override TRight Right => _value;
        public override bool IsLeft => false;
        public override bool IsRight => true;
    }
}