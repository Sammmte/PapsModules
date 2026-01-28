using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using System;

namespace Paps.UnityExtensions
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SerializableGuid : IComparable, IComparable<SerializableGuid>, IComparable<Guid>, IEquatable<SerializableGuid>
    {
        #region Fields

        [SerializeField, HideInInspector]
        public int A;
        [SerializeField, HideInInspector]
        public short B;
        [SerializeField, HideInInspector]
        public short C;
        [SerializeField, HideInInspector]
        public byte D;
        [SerializeField, HideInInspector]
        public byte E;
        [SerializeField, HideInInspector]
        public byte F;
        [SerializeField, HideInInspector]
        public byte G;
        [SerializeField, HideInInspector]
        public byte H;
        [SerializeField, HideInInspector]
        public byte I;
        [SerializeField, HideInInspector]
        public byte J;
        [SerializeField, HideInInspector]
        public byte K;

        #endregion

        #region CONSTRUCTOR

        public SerializableGuid(int a, short b, short c, byte d, byte e, byte f, byte g, byte h, byte i, byte j, byte k)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
            this.E = e;
            this.F = f;
            this.G = g;
            this.H = h;
            this.I = i;
            this.J = j;
            this.K = k;
        }

        public SerializableGuid(ulong high, ulong low)
        {
            A = (int)(high >> 32);
            B = (short)(high >> 16);
            C = (short)high;
            D = (byte)(low >> 56);
            E = (byte)(low >> 48);
            F = (byte)(low >> 40);
            G = (byte)(low >> 32);
            H = (byte)(low >> 24);
            I = (byte)(low >> 16);
            J = (byte)(low >> 8);
            K = (byte)low;
        }

        #endregion

        #region Methods

        public bool IsEmpty() => A == 0 && B == 0 && C == 0 && D == 0 && E == 0 && F == 0 && G == 0 && H == 0 && I == 0 && J == 0 && K == 0;

        public bool HasValue() => A != 0 || B != 0 || C != 0 || D != 0 || E != 0 || F != 0 || G != 0 || H != 0 || I != 0 || J != 0 || K != 0;

        public void ToHighLow(out ulong high, out ulong low)
        {
            high = (ulong)A << 32;
            high |= (ulong)B << 16;
            high |= (ulong)C << 16;
            low = (ulong)D << 56;
            low |= (ulong)E << 48;
            low |= (ulong)F << 40;
            low |= (ulong)G << 32;
            low |= (ulong)H << 24;
            low |= (ulong)I << 16;
            low |= (ulong)J << 8;
            low |= (ulong)K;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ToByteArray() => this.ToGuid().ToByteArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => this.ToGuid().ToString();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format) => this.ToGuid().ToString(format);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Guid ToGuid() => new Guid(A, B, C, D, E, F, G, H, I, J, K);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SerializableGuid guid) => guid.ToGuid().Equals(this.ToGuid());
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Guid guid) => guid.Equals(this.ToGuid());

        public override bool Equals(object obj)
        {
            if (obj is SerializableGuid sg)
            {
                return this.ToGuid() == sg.ToGuid();
            }
            else if (obj is Guid g)
            {
                return this.ToGuid() == g;
            }
            else
            {
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => this.ToGuid().GetHashCode();


        int IComparable.CompareTo(object obj)
        {
            if (obj is SerializableGuid sg)
            {
                return this.ToGuid().CompareTo(sg.ToGuid());
            }
            else if (obj is Guid g)
            {
                return this.ToGuid().CompareTo(g);
            }
            else
            {
                return 1;
            }
        }

        int IComparable<SerializableGuid>.CompareTo(SerializableGuid other)
        {
            return this.ToGuid().CompareTo(other.ToGuid());
        }

        int IComparable<Guid>.CompareTo(Guid other)
        {
            return this.ToGuid().CompareTo(other);
        }

        #endregion

        #region Static Utils

        public static readonly SerializableGuid Empty = default;

        public static SerializableGuid NewGuid()
        {
            return System.Guid.NewGuid();
        }

        public static SerializableGuid Parse(string input)
        {
            return System.Guid.Parse(input);
        }

        public static bool TryParse(string input, out SerializableGuid result)
        {
            Guid output;
            if (Guid.TryParse(input, out output))
            {
                result = output;
                return true;
            }
            else
            {
                result = default(SerializableGuid);
                return false;
            }
        }

        public static bool Coerce(object input, out SerializableGuid result)
        {
            Guid guid;
            if (CoerceGuid(input, out guid))
            {
                result = guid;
                return true;
            }
            else
            {
                result = default(SerializableGuid);
                return false;
            }
        }

        public static bool CoerceGuid(object input, out Guid result)
        {
            switch (input)
            {
                case Guid guid:
                    result = guid;
                    return true;
                case SerializableGuid sguid:
                    result = sguid.ToGuid();
                    return true;
                case string s:
                    return Guid.TryParse(s, out result);
                case object o:
                    return Guid.TryParse(o.ToString(), out result);
                default:
                    result = default(SerializableGuid);
                    return false;
            }
        }

        public static implicit operator Guid(SerializableGuid guid)
        {
            return guid.ToGuid();
        }

        public unsafe static implicit operator SerializableGuid(Guid guid)
        {
            return *(SerializableGuid*)&guid;
        }

        public static bool operator ==(SerializableGuid a, SerializableGuid b)
        {
            return a.ToGuid() == b.ToGuid();
        }

        public static bool operator ==(Guid a, SerializableGuid b)
        {
            return a == b.ToGuid();
        }

        public static bool operator ==(SerializableGuid a, Guid b)
        {
            return a.ToGuid() == b;
        }

        public static bool operator !=(SerializableGuid a, SerializableGuid b)
        {
            return a.ToGuid() != b.ToGuid();
        }

        public static bool operator !=(Guid a, SerializableGuid b)
        {
            return a != b.ToGuid();
        }

        public static bool operator !=(SerializableGuid a, Guid b)
        {
            return a.ToGuid() != b;
        }

        #endregion
    }
}
