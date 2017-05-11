using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Incrementors
{
    public sealed class Incrementor
    {
        private Incrementor _related;

        public Incrementor(int min, int max, int increment = 1)
             : this(null, min, max, increment)
        {
        }

        public Incrementor(Incrementor related, int min, int max, int increment = 1)
            : this(related, min, max, increment, min)
        {
        }

        private Incrementor(Incrementor related, int min, int max, int increment, int currentValue)
        {
            _related = related;
            Minimum = min;
            Maximum = max;
            Increment = increment;
            Value = currentValue;
            CanIncrement = IsIncrementable(); 
        }

        public int Value { get; }
        public int Increment { get; }
        public int Minimum { get; }
        public int Maximum { get; }
        public bool CanIncrement { get; }

        private bool IsIncrementable()
        {
            if (Value < Maximum)
                return true;
            if (_related == null)
                return false;

            return _related.IsIncrementable();
        }

        public Incrementor SetParent(Incrementor parentIncrementor)
        {
            return new Incrementor(parentIncrementor, Minimum, Maximum, Increment);
        }

        public Incrementor IncrementValue()
        {
            if (!CanIncrement)
                throw new InvalidOperationException();

            if (Value == Maximum)
            {
                if (_related != null)
                    _related = _related.IncrementValue();
                return new Incrementor(_related, Minimum, Maximum, Increment);
            }

            return new Incrementor(_related, Minimum, Maximum, Increment, Value + Increment);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (_related != null)
                sb.Append(_related.ToString());
            sb.Append(Value);

            return sb.ToString();
        }

        public static Incrementor CreateCombined(IEnumerable<Incrementor> incrementors)
        {
            return incrementors.Aggregate<Incrementor, Incrementor>(null, (a, i) => { return (a == null) ? i : i.SetParent(a); });
        }
    }
}
