using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    static class Constants
    {
        public static readonly float EPSILON = Mathf.Epsilon;
        public static readonly int DEFAULT_INTERVALS = 10;
        public static readonly int DEFAULT_MAX_ITERATIONS = 15;
        public static readonly float PI = Mathf.PI;
    }

    static public class Math
    {
        public static int Binomial(int n, int k)
        {
            int val = 1;
            for (int i = 1; i <= k; i++)
            {
                val *= n + 1 - i;
                val /= i;
            }
            return val;
        }

        public static bool IsWithinZeroAndOne(float x)
        {
            return x >= -Constants.EPSILON && x <= (1.0 + Constants.EPSILON);
        }
    }

    public class BinomialCoefficients
    {
        private List<int> mCoefficients;
        private int N = 0;
        public BinomialCoefficients(int n)
        {
            N = n;
            int center = N / 2;
            int k = 0;

            mCoefficients = new List<int>(N + 1);
            for(int i = 0; i < N + 1; ++i)
            {
                mCoefficients.Add(0);
            }

            while (k <= center)
            {
                mCoefficients[k] = Math.Binomial(N, k);
                k++;
            }

            // Utilize the symmetrical nature of the binomial coefficients.
            while (k <= N)
            {
                mCoefficients[k] = mCoefficients[N - k];
                k++;
            }
        }

        public int Size()
        {
            return N + 1;
        }

        public int this[int idx]
        {
            get { return mCoefficients[idx]; }
        }

    }

    public class PolynomialPair
    {
        public int t = 0;
        public int one_minus_t = 0;

        public float ValueAt(float tt)
        {
            return (Mathf.Pow(1.0f - tt, one_minus_t) * Mathf.Pow(tt, (float)(t)));
        }
    };

    public class PolynomialCoefficients
    {
        private int N = 0;
        private List<PolynomialPair> mPolynomialPairs;
        public PolynomialCoefficients(int n)
        {
            N = n;
            mPolynomialPairs = new List<PolynomialPair>(N + 1);
            for (int i = 0; i < N + 1; ++i)
            {
                mPolynomialPairs.Add(new PolynomialPair());
            }
            for (int i = 0; i <= N; i++)
            {
                mPolynomialPairs[i].t = i;
                mPolynomialPairs[i].one_minus_t = N - i;
            }
        }

        public float ValueAt(int pos, float t)
        {
            return mPolynomialPairs[pos].ValueAt(t);
        }

        public PolynomialPair this[int idx]
        {
            get { return mPolynomialPairs[idx]; }
        }
    }

    public class Vec2
    {
        public float x = 0.0f;
        public float y = 0.0f;
        public Vec2()
        { }

        public Vec2(float xx, float yy)
        {
            x = xx;
            y = yy;
        }
        public Vec2(Vec2 other)
        {
            x = other.x;
            y = other.y;
        }

        public Vec2(float xx, float yy, bool normalize)
        {
            x = xx;
            y = yy;
            if (normalize)
            {
                Normalize();
            }
        }


        public void Set(float xx, float yy)
        {
            x = xx;
            y = yy;
        }

        public void Set(Vec2 other)
        {
            x = other.x;
            y = other.y;
        }

        public float Length()
        {
            return UnityEngine.Mathf.Sqrt(x * x + y * y);
        }

        public void Normalize()
        {
            float len = Length();
            x /= len;
            y /= len;
        }

        public void Translate(float dx, float dy)
        {
            x += dx;
            y += dy;
        }

        public void Translate(Vec2 distance)
        {
            x += distance.x;
            y += distance.y;
        }

        public void Rotate(float angle, Vec2 pivot)
        {
            float s = UnityEngine.Mathf.Sin(angle);
            float c = UnityEngine.Mathf.Cos(angle);

            x -= pivot.x;
            y -= pivot.y;

            float xnew = x * c - y * s;
            float ynew = x * s + y * c;

            x = xnew + pivot.x;
            y = ynew + pivot.y;
        }

        public float Angle()
        {
            return UnityEngine.Mathf.Atan2(y, x);
        }

        public float AngleDeg()
        {
            return Angle() * 180.0f / Constants.PI;
        }

        public float this[int axis]
        {
            get
            { 
                switch (axis)
                {
                case 0:
                    return x;
                case 1:
                    return y;
                default:
                    return 0;
                }
            }
            set
            {
                switch (axis)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        x = value;
                        break;
                }
            }
        }


        public bool Equals(Vec2 other)
        {
            bool equals = true;
            for (int axis = 0; axis<Vec2.Size; axis++)
            {
                if (Mathf.Abs((this)[axis] - other [axis]) >= Constants.EPSILON)
                {
                    equals = false;
                    break;
                }
            }
            return equals;
        }


        public bool IsWithinZeroAndOne()
        {
            return Math.IsWithinZeroAndOne(x) && Math.IsWithinZeroAndOne(y);
        }

        static public Vec2 operator + (Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        static public Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x - b.x, a.y - b.y);
        }

        static public Vec2 operator *(Vec2 a, int val)
        {
            return new Vec2(a.x * val, a.y * val);
        }

        public static int Size = 2;
    };

    public class ExtremeValue
    {
        private float t;
        private int axis;

        public ExtremeValue(float t_, int axis_)
        {
            t = t_;
            axis = axis_;
        }

        public bool Equals(ExtremeValue other)
        {
            return axis == other.axis && Mathf.Abs(t - other.t) < Constants.EPSILON;
        }
    };

    public class Bezier
    {
        public int N { get; set; } = 0;
        protected List<Vec2> mControlVec2s;
        BinomialCoefficients binomialCoefficients;
        PolynomialCoefficients polynomialCoefficients;

        public Bezier(int N_)
        {
            N = N_;
            mControlVec2s = new List<Vec2>(N + 1);
            binomialCoefficients = new BinomialCoefficients(N);
            polynomialCoefficients = new PolynomialCoefficients(N);
        }

        public Bezier(List<Vec2> controlVec2s)
        {
            N = controlVec2s.Count - 1;
            binomialCoefficients = new BinomialCoefficients(N);
            polynomialCoefficients = new PolynomialCoefficients(N);
            mControlVec2s = new List<Vec2>(N + 1);

            for (int i = 0; i < controlVec2s.Count; i++)
            {
                mControlVec2s.Add(new Vec2(controlVec2s[i]));
            }
        }

        public Bezier(Bezier other)
        {
            N = other.N;
            binomialCoefficients = new BinomialCoefficients(N);
            polynomialCoefficients = new PolynomialCoefficients(N);
            mControlVec2s = new List<Vec2>(N + 1);
            for (int i = 0; i < other.mControlVec2s.Count; i++)
            {
                mControlVec2s.Add(new Vec2(other[i]));
            }
        }

        public Vec2 this[int idx]
        {
            get { return mControlVec2s[idx]; }
        }

        // The order of the bezier curve.
        public int Order()
        {
            return N;
        }

        // Number of control points.
        public int Size()
        {
            return N + 1;
        }

        public Bezier Derivative()
        {
            List<Vec2> derivativeWeights = new List<Vec2>(N);
            for (int i = 0; i < N; i++)
            {
                derivativeWeights.Add(new Vec2((mControlVec2s[i + 1] - mControlVec2s[i]) * N));
            }

            return new Bezier(derivativeWeights);
        }

        public float ValueAt(float t, int axis)
        {
            float sum = 0;
            for (int n = 0; n < N + 1; n++)
            {
                sum += binomialCoefficients[n] * polynomialCoefficients[n].ValueAt(t) * mControlVec2s[n][axis];
            }
            return sum;
        }

        public Vec2 ValueAt(float t)
        {
            Vec2 p = new Vec2();
            p.x = ValueAt(t, 0);
            p.y = ValueAt(t, 1);
            return p;
        }
    }
} 
// namespace Bezier