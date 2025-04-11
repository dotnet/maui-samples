namespace Animations;

public static class CustomEasing
{
    
    public static readonly Easing BounceInOut = 
        new Easing(t => 
            (t < 0.5) 
                ? (1 - Easing.BounceOut.Ease(1 - (2 * t))) / 2
                : (1 + Easing.BounceOut.Ease((2 * t) - 1)) / 2
        );
    
    public static readonly Easing QuadIn = 
        new Easing(t => 
            t * t
        );
    
    public static readonly Easing QuadOut = 
        new Easing(t => 
            1 - (1-t) * (1-t)
        );
    
    public static readonly Easing QuadInOut = 
        new Easing(t => 
            (t < 0.5) 
                ? 2 * t * t
                : 1 - Math.Pow(-2 * t + 2, 2) / 2
        );
    
    public static readonly Easing QuartIn = 
        new Easing(t => 
            t * t * t * t
        );
    
    public static readonly Easing QuartOut = 
        new Easing(t => 
            1 - Math.Pow(1-t,4)
        );
    
    public static readonly Easing QuartInOut = 
        new Easing(t => 
            (t < 0.5) 
                ? 8 * t * t * t * t
                : 1 - Math.Pow(-2 * t + 2, 4) / 2
        );
    
    public static readonly Easing ExpoIn = 
        new Easing(t =>
            (t == 0)
            ? 0
            : Math.Pow(2, 10 * t - 10)
        );
    
    public static readonly Easing ExpoOut = 
        new Easing(t => 
            (t == 1)
                ? 1
                : Math.Pow(2, -10 * t)
        );
    
    public static readonly Easing ExpoInOut = 
        new Easing(t => 
            (t == 0)
                ? 0
                : (t == 1)
                    ? 1
                : (t < 0.5) ? Math.Pow(2, 20 * t - 10) / 2 : (2 - Math.Pow(2, -20 * t + 10)) / 2
                
        );
    
    private static double c1 = 1.70158;
    private static double c2 = c1 + 1.525;
    private static double c3 = c1 + 1; 
    private static double c4 = (2 * Math.PI) / 3;
    private static double c5 = (2 * Math.PI) / 4.5;
    
    public static readonly Easing BackIn = 
        new Easing(t =>
            c3 * t * t * t - c1 * t * t
        );
    
    public static readonly Easing BackOut = 
        new Easing(t => 
            1 + c3 * Math.Pow(t-1,3) + c1 * Math.Pow(t-1,2)
        );
    
    public static readonly Easing BackInOut = 
        new Easing(t => 
            (t < 0.5)
                ? (Math.Pow(2*t, 2)*((c2 + 1)*2*t-c2)) / 2
                : (Math.Pow(2*t-2,2) * ((c2+1) * (t*2-2) + c2)+2)/2
                
        );
    
    public static readonly Easing QuintIn = 
        new Easing(t =>
            t*t*t*t*t
        );
    
    public static readonly Easing QuintOut = 
        new Easing(t => 
            1 - Math.Pow(1-t,5)
        );
    
    public static readonly Easing QuintInOut = 
        new Easing(t => 
            (t < 0.5)
                ? 16*t*t*t*t*t
                : 1 - Math.Pow(-1 * t + 2,5)
                
        );
    
    public static readonly Easing CircIn = 
        new Easing(t =>
            1 - Math.Sqrt(1-Math.Pow(t,2))
        );
    
    public static readonly Easing CircOut = 
        new Easing(t => 
            Math.Sqrt(1-Math.Pow(t-1,2))
        );
    
    public static readonly Easing CircInOut = 
        new Easing(t => 
            (t < 0.5)
                ? (1 - Math.Sqrt(1 - Math.Pow(2 * t, 2))) / 2
                : (Math.Sqrt(1 - Math.Pow(-2 * t + 2, 2)) + 1) / 2
                
        );
    
    public static readonly Easing ElasticIn = 
        new Easing(t =>
            (t==0)
                ? 0
                : t == 1
                    ? 1
                    : -Math.Pow(2, 10 * t - 10) * Math.Sin((t * 10 - 10.75) * c4)
        );
    
    public static readonly Easing ElasticOut = 
        new Easing(t => 
            (t==0)
                ? 0
                : t == 1
                    ? 1
                    : Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1
        );
    
    public static readonly Easing ElasticInOut = 
        new Easing(t => 
            (t == 0)
                ? 0
                : t == 1
                    ? 1
                    : t < 0.5
                        ? -(Math.Pow(2, 20 * t - 10) * Math.Sin((20 * t - 11.125) * c5)) / 2
                        : (Math.Pow(2, -20 * t + 10) * Math.Sin((20 * t - 11.125) * c5)) / 2 + 1
                
        );
    

    
}