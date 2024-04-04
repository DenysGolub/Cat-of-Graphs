using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.TestingPart
{
    public interface IStateParameter
    {
        string State { get; }
        object Value { get; set; }
    }

    public abstract class StateParameter<T> : IStateParameter, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string State { get; set; }

        public T Value
        {
            get { return _v; }
            set
            {
                if ((_v as IEquatable<T>)?.Equals(value) == true || ReferenceEquals(_v, value) || _v?.Equals(value) == true)
                    return;

                _v = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        private T _v;

        object IStateParameter.Value
        {
            get { return this.Value; }
            set { this.Value = (T)value; }
        }
    }

    public class BoolStateParameter : StateParameter<bool>
    { }

    public class TextStateParameter : StateParameter<string>
    { }

    public class ChoiceStateParameter : StateParameter<object>
    {
        public Array Choices { get; set; }
    }

    public class AdjMatrixStateParameter : StateParameter<bool>
    {
        private int[,] _matrix = new int[0, 0];
        public int[,] Matrix
        {
            get
            {
                return _matrix;
            }
            set
            {
                _matrix = value;
            }
        }
    }




}
