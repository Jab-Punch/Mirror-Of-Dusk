using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
using UnityEditor;

public class InputReader : MonoBehaviour {

    public List<string> inputName;                  //List of available input buttons
    public List<InputPressData>[] inputRead;        //List of current buttons "PRESSED" that are in use
    public List<InputPressData>[] inputRelease;     //List of current buttons "RELEASED" that are to be confirmed
    public Dictionary<string, System.TimeSpan>[] inputHold;     //List of current buttons "HELD" that are in use
    public Dictionary<string, int>[] inputHoldCount;            //List of current buttons "HELD" that are pressed by more than one of the same key
    public class InputPressData
    {
        public string ipName;           //Name of input
        public int ipDur = 0;           //*DISCONTINUED!!*  Duration for input to be read
        public bool ipRemove = false;   //*DISCONTINUED!!*  Trigger for input to be removed
    }
    public bool[] axisKeyOn;            //Checks if the keyboard arrows are being used

    private System.TimeSpan[] time;                     //Current time for each player
    private System.TimeSpan fullTime;                   //Current overall time
    //private System.TimeSpan[] timeSinceLastUpdate;
    private System.TimeSpan timeSinceLastFullUpdate;
    //private bool isPaused = false;

    private static readonly System.TimeSpan BufferTimeOut = System.TimeSpan.FromMilliseconds(1000);     //Time before an input buffer is cleared
    private static readonly System.TimeSpan BufferSubTimeOut = System.TimeSpan.FromMilliseconds(400);   //Time before an input buffer is disabled from use
    private static readonly System.TimeSpan MergeInputTime = System.TimeSpan.FromMilliseconds(20);      //Time before all current inputs are merged into a simultaneous press
    private static readonly System.TimeSpan NextInputTime = System.TimeSpan.FromMilliseconds(250);      //Time before the next required input can be met

    private System.TimeSpan[] _lastBufferUpdateTime;
    private System.TimeSpan[] _mergeInputTime;
    
    private BufferInput[] _bufferInput;         //List of current inputs in buffer order.
    //private BufferHold[] _bufferHold;           //List of current inputs that are held.
    private List<InputInfo>[] _mergeBuffer;     //List of current inputs ready to merge.
    //private List<int>[] _usedCode;              //List of code keys for each input sequence that has been used.

    private List<InputType>[] _keys;            //List of available keys to add before next merge.

    //Enum list for types of move command labels
    public enum MoveType
    {
        MoveRight, MoveLeft, MoveUp, MoveDown, MoveUp_Right, MoveUp_Left, MoveDown_Right, MoveDown_Left, Confirm, Cancel, Unknown, Hidden, Hidden2, Piledriver, LightOn, LightOff, LightOffLong
    }

    //Enum list for types of available input buttons. Note:R_ is for Right Analog Stick. K_ is for Keyboard Arrows.
    public enum InputType
    {
        Up, Down, Left, Right, Up_Left, Up_Right, Down_Left, Down_Right,
        R_Up, R_Down, R_Left, R_Right, R_Up_Left, R_Up_Right, R_Down_Left, R_Down_Right,
        K_Up, K_Down, K_Left, K_Right, K_Up_Left, K_Up_Right, K_Down_Left, K_Down_Right,
        Start, Select, A, B, C, D, L1, L2, R1, R2, L3, R3
    }

    //Enum list for types of available input commands that can halt a command if wrongly pressed.
    public enum InputSkip
    {
        Start, Select, A, B, C, D, L1, L2, R1, R2, L3, R3
    }

    //Phase of current input
    public enum InputStyle
    {
        Press, Hold, Release
    }

    //General data of current input
    public class InputInfo
    {
        private InputType _inputType;       //Name of input
        private InputStyle _styleType;      //Phase of input
        private System.TimeSpan _duration;  //Duration of input when held
        private bool _combine;              //Can this input merge with a previous input

        public InputInfo(InputType inputType, InputStyle styleType, int duration)
        {
            _inputType = inputType;
            _styleType = styleType;
            _duration = System.TimeSpan.FromMilliseconds(duration);
            _combine = false;
        }

        public InputInfo(InputType inputType, InputStyle styleType, System.TimeSpan duration)
        {
            _inputType = inputType;
            _styleType = styleType;
            _duration = duration;
            _combine = false;
        }

        public InputInfo(InputType inputType, InputStyle styleType, int duration, bool combine)
        {
            _inputType = inputType;
            _styleType = styleType;
            _duration = System.TimeSpan.FromMilliseconds(duration);
            _combine = combine;
        }

        public InputType Value
        {
            get { return _inputType; }
        }

        public InputStyle Style
        {
            get { return _styleType; }
        }

        public System.TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public bool Combine
        {
            get { return _combine; }
        }
    }
    
    //The order of inputs needed to execute a command
    public struct InputSequence
    {
        private readonly List<List<InputInfo>> _sequence;   //Order of input buttons
        private readonly List<bool> _tight;                 //To confirm if current input counts as a possible wrong press
        private readonly List<bool> _precise;               //To confirm if button does not excuse wrong follow-up inputs
        private readonly bool _hold;                        //To confirm if certain buttons must be held to prevent input dropping
        private readonly bool _repeat;                      //To confirm if this input can be repeated via last button

        public InputSequence(List<List<InputInfo>> sequence, bool hold, bool repeat)
        {
            _sequence = sequence;
            _tight = new List<bool>();
            _precise = new List<bool>();
            _hold = hold;
            _repeat = repeat;

            LabelTight();
            for (var i = 0; i < sequence.Count; i++)
            {
                _precise.Add(false);
            }
        }

        public InputSequence(List<List<InputInfo>> sequence, List<int> precise, bool hold, bool repeat)
        {
            _sequence = sequence;
            _tight = new List<bool>();
            _precise = new List<bool>();
            _hold = hold;
            _repeat = repeat;

            LabelTight();
            LabelPrecise(precise);
        }

        public InputSequence(InputInfo[,] sequence, bool hold, bool repeat)
        {
            _sequence = new List<List<InputInfo>>();
            for (var i = 0; i < sequence.GetLength(0); i++)
            {
                var input = new List<InputInfo>();
                for (var j = 0; j < sequence.GetLength(1); j++)
                {
                    input.Add(sequence[i, j]);
                }
                _sequence.Add(input);
            }
            _tight = new List<bool>();
            _precise = new List<bool>();
            _hold = hold;
            _repeat = repeat;

            LabelTight();
            for (var i = 0; i < sequence.GetLength(0); i++)
            {
                _precise.Add(false);
            }
        }

        public InputSequence(InputInfo[,] sequence, List<int> precise, bool hold, bool repeat)
        {
            _sequence = new List<List<InputInfo>>();
            for (var i = 0; i < sequence.GetLength(0); i++)
            {
                var input = new List<InputInfo>();
                for (var j = 0; j < sequence.GetLength(1); j++)
                {
                    input.Add(sequence[i, j]);
                }
                _sequence.Add(input);
            }
            _tight = new List<bool>();
            _precise = new List<bool>();
            _hold = hold;
            _repeat = repeat;

            LabelTight();
            LabelPrecise(precise);
        }

        public List<List<InputInfo>> Value
        {
            get { return _sequence; }
        }

        public List<bool> Tight
        {
            get { return _tight; }
        }

        public List<bool> Precise
        {
            get { return _precise; }
        }

        public bool Hold
        {
            get { return _hold; }
        }

        public bool Repeat
        {
            get { return _repeat; }
        }

        public override string ToString()
        {
            /*var inputSequenceStrings = _sequence.Select(input =>
            {
                var inputString = input.Select(e => e.ToString());
                return "{" + string.Join(", ", inputString.ToArray()) + "}";
            });
            return "[ " + string.Join(", ", inputSequenceStrings.ToArray()) + " ]";*/
            string word = "[ ";
            for (int i = 0; i < _sequence.Count; i++)
            {
                word += _sequence[i].ToString();
                word += ", ";
            }
            word = word.Substring(0, word.Length - 2);
            word += " ]";
            return word;
        }

        //Assign the inputs set as possible wrong inputs
        private void LabelTight()
        {
            for (int i = 0; i < _sequence.Count; i++)
            {
                _tight.Add(false);
                bool brkOut = false;
                for (int j = 0; j < _sequence[i].Count; j++)
                {
                    foreach (InputSkip skip in (InputSkip[])System.Enum.GetValues(typeof(InputSkip)))
                    {
                        if (_sequence[i][j].Value.ToString() == skip.ToString())
                        {
                            brkOut = true;
                            break;
                        }
                    }
                    if (brkOut)
                    {
                        _tight[i] = true;
                        break;
                    }
                }
            }
        }

        //Assign the inputs meant to read for wrong inputs
        private void LabelPrecise(List<int> precise)
        {
            for (var i = 0; i < precise.Count; i++)
            {
                _precise.Add(false);
                if (precise[i] > 0)
                {
                    _precise[i] = true;
                }
            }
        }
    }

    //The buffer list of current inputs
    public struct BufferInput
    {
        private List<List<InputInfo>> _inputSet;        //List of input names
        private List<System.TimeSpan> _bufferCount;     //Duration of inputs when held
        private List<bool> _used;                       //Confirms the input that was last used in a successful command
        private List<List<string>> _usedInputs;         //Identifies the type of move last used

        public BufferInput(List<List<InputInfo>> inputSet, List<System.TimeSpan> bufferCount)
        {
            _inputSet = inputSet;
            _bufferCount = bufferCount;
            _used = new List<bool>();
            _usedInputs = new List<List<string>>();
        }

        public BufferInput(List<List<InputInfo>> inputSet)
        {
            _inputSet = inputSet;
            _bufferCount = new List<System.TimeSpan>();
            _used = new List<bool>();
            _usedInputs = new List<List<string>>();
        }

        public BufferInput(InputInfo[,] inputSet, List<System.TimeSpan> bufferCount)
        {
            _inputSet = new List<List<InputInfo>>();
            for (var i = 0; i < inputSet.GetLength(0); i++)
            {
                var input = new List<InputInfo>();
                for (var j = 0; j < inputSet.GetLength(1); j++)
                {
                    input.Add(inputSet[i, j]);
                }
                _inputSet.Add(input);
            }
            _bufferCount = bufferCount;
            _used = new List<bool>();
            _usedInputs = new List<List<string>>();
        }

        public List<List<InputInfo>> Value
        {
            get { return _inputSet; }
            set { this.Value = value; }
        }

        public List<System.TimeSpan> Buffer
        {
            get { return _bufferCount; }
            set { this.Buffer = value; }
        }

        public List<bool> Used
        {
            get { return _used; }
            set { _used = value; }
        }

        public List<List<string>> UsedInputs
        {
            get { return _usedInputs; }
            set { this.UsedInputs = value; }
        }

        public override string ToString()
        {
            /*var inputSequenceStrings = _inputSet.Select(input =>
            {
                var inputString = input.Select(e => e.ToString());
                return "{" + string.Join(", ", inputString.ToArray()) + "}";
            });
            return "[ " + string.Join(", ", inputSequenceStrings.ToArray()) + " ]";*/
            string word = "[ ";
            for (int i = 0; i < _inputSet.Count; i++)
            {
                word += _inputSet[i].ToString();
                word += ", ";
            }
            word = word.Substring(0, word.Length - 2);
            word += " ]";
            return word;
        }
    }

    //The buffer list of current inputs on hold
    public struct BufferHold
    {
        private List<InputInfo> _inputSet;          //List of current input names
        private List<System.TimeSpan> _holdCount;   //Duration of how long the input has been held

        public BufferHold(List<InputInfo> inputSet, List<System.TimeSpan> holdCount)
        {
            _inputSet = inputSet;
            _holdCount = holdCount;
        }

        public BufferHold(List<InputInfo> inputSet)
        {
            _inputSet = inputSet;
            _holdCount = new List<System.TimeSpan>();
        }

        public BufferHold(InputInfo[] inputSet, List<System.TimeSpan> holdCount)
        {
            _inputSet = new List<InputInfo>();
            for (var i = 0; i < inputSet.Length; i++)
            {
                _inputSet.Add(inputSet[i]);
            }
            _holdCount = holdCount;
        }

        public List<InputInfo> Value
        {
            get { return _inputSet; }
            set { this.Value = value; }
        }

        public List<System.TimeSpan> Buffer
        {
            get { return _holdCount; }
            set { this.Buffer = value; }
        }

        public override string ToString()
        {
            /*var inputSequenceStrings = _inputSet.Select(e => e.ToString());
            var holdSequenceStrings = _holdCount.Select(e => e.ToString());
            return "[ " + string.Join(", ", inputSequenceStrings.ToArray()) + " ; " + string.Join(", ", holdSequenceStrings.ToArray()) + " ]";*/
            string word = "[ ";
            for (int i = 0; i < _inputSet.Count; i++)
            {
                word += _inputSet[i].ToString();
                word += ", ";
            }
            word = word.Substring(0, word.Length - 2);
            word += " ]";
            return word;
        }
    }

    //Available list of commands
    public static class UniversalMoveData
    {
        public static readonly Dictionary<MoveType, InputSequence> Moves = new Dictionary<MoveType, InputSequence>();

        static UniversalMoveData()
        {
            Moves.Add(MoveType.LightOffLong, new InputSequence(new[,] { { new InputInfo(InputType.A, InputStyle.Hold, 1000) }, { new InputInfo(InputType.A, InputStyle.Release, 0) }, { new InputInfo(InputType.B, InputStyle.Press, 0, true) } }, false, false));
            Moves.Add(MoveType.LightOff, new InputSequence(new[,] { { new InputInfo(InputType.A, InputStyle.Release, 0) } }, false, false));
            Moves.Add(MoveType.LightOn, new InputSequence(new[,] { { new InputInfo(InputType.A, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.Piledriver, new InputSequence(new[,] { { new InputInfo(InputType.Left, InputStyle.Press, 0) }, { new InputInfo(InputType.Down, InputStyle.Press, 0) }, { new InputInfo(InputType.Right, InputStyle.Press, 0) }, { new InputInfo(InputType.Up, InputStyle.Press, 0) }, { new InputInfo(InputType.Left, InputStyle.Press, 0) }, { new InputInfo(InputType.Down, InputStyle.Press, 0) }, { new InputInfo(InputType.Right, InputStyle.Press, 0) }, { new InputInfo(InputType.Up, InputStyle.Press, 0) }, { new InputInfo(InputType.D, InputStyle.Press, 0, true) } }, false, false));
            Moves.Add(MoveType.Hidden2, new InputSequence(new[,] { { new InputInfo(InputType.D, InputStyle.Press, 0) }, { new InputInfo(InputType.C, InputStyle.Press, 0) }, { new InputInfo(InputType.C, InputStyle.Press, 0) }, { new InputInfo(InputType.D, InputStyle.Press, 0) } }, false, false));
            Moves.Add(MoveType.Hidden, new InputSequence(new[,] { { new InputInfo(InputType.C, InputStyle.Press, 0) }, { new InputInfo(InputType.C, InputStyle.Press, 0) } },
                new List<int> { 0, 1 }, false, true));
            Moves.Add(MoveType.Unknown, new InputSequence(new[,] { { new InputInfo(InputType.C, InputStyle.Press, 0), new InputInfo(InputType.D, InputStyle.Press, 0) } }, false, false));
            Moves.Add(MoveType.Cancel, new InputSequence(new[,] { { new InputInfo(InputType.C, InputStyle.Press, 0) } }, false, false));
            Moves.Add(MoveType.Confirm, new InputSequence(new[,] { { new InputInfo (InputType.D, InputStyle.Press, 0) } }, false, false));
            Moves.Add(MoveType.MoveDown_Left, new InputSequence(new[,] { { new InputInfo(InputType.Down_Left, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveDown_Right, new InputSequence(new[,] { { new InputInfo(InputType.Down_Right, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveUp_Left, new InputSequence(new[,] { { new InputInfo(InputType.Up_Left, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveUp_Right, new InputSequence(new[,] { { new InputInfo(InputType.Up_Right, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveDown, new InputSequence(new[,] { { new InputInfo(InputType.Down, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveUp, new InputSequence(new[,] { { new InputInfo(InputType.Up, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveLeft, new InputSequence(new[,] { { new InputInfo(InputType.Left, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveRight, new InputSequence(new[,] { { new InputInfo(InputType.Right, InputStyle.Hold, 0) } }, true, false));
        }
    }

    //Match the selected input with the input requirements
    private bool MatchSimultaneousLastKeys(List<InputInfo> bufferedInput, List<InputInfo> template)
    {
        int fCount = template.Count;        //The count of inputs in a merge that need to be found

        for (int j = 0; j < template.Count; j++)
        {
            for (int i = 0; i < bufferedInput.Count; i++)
            {
                if (bufferedInput[i].Value == template[j].Value && bufferedInput[i].Style == template[j].Style && bufferedInput[i].Duration >= template[j].Duration)
                {
                    fCount--;   //If a matched input has been found, reduce the counter
                    break;
                }
            }
        }

        //Once all matches have been found, return true.
        if (fCount <= 0)
        {
            return true;
        }

        return false;
    }

    private bool MatchHoldKeysForRelease(List<InputInfo> bufferedInput, List<InputInfo> template)
    {
        for (int j = 0; j < template.Count; j++)
        {
            if (template[j].Style == InputStyle.Hold)
            {
                for (int i = 0; i < bufferedInput.Count; i++)
                {
                    if (bufferedInput[i].Value == template[j].Value && bufferedInput[i].Style == InputStyle.Release)
                    {
                        return true;   //If a matched input has been found, but is a release from the buffer list, return true.
                    }
                }
            }
        }

        return false;
    }

    //Matches the buffer list with the whole input sequence via backwards tracking
    public bool MatchesLast(int pCode, InputSequence inputSequence, string input)
    {
        // If the move is longer than the buffer, it can't possibly match.
        if (_bufferInput[pCode].Value.Count < inputSequence.Value.Count)
        {
            return false;
        }

        int setUsed = -1;           //Trace the array key of any input that is set to be labelled as USED.
        int findCount = 1;          //Increment for each piece of the input command that is found in order.
        bool preciseSet = false;    //For confirming if the input restricts wrong follow-ups.
        bool cancelCount = false;   //Stop the search for the remaining inputs in the buffer list once a match is completely found.
        System.TimeSpan nextFoundTime;      //The time remaining for the next input to be followed up.
        
        // Loop backwards to match against the most recent input.
        for (int i = 1; i <= _bufferInput[pCode].Value.Count; ++i)
        {
            var bufferedValue = _bufferInput[pCode].Value[_bufferInput[pCode].Value.Count - i];     //Value of current input in buffer list.
            var moveValue = inputSequence.Value[inputSequence.Value.Count - findCount];             //Value of current input in the required input command sequence.

            //If the next first input is found before the last used input in the buffer list, cancel the move.
            if (findCount <= 1 && _bufferInput[pCode].Used[_bufferInput[pCode].Value.Count - i] == true && !inputSequence.Hold)
            {
                return false;
            }

            if (_bufferInput[pCode].Used[_bufferInput[pCode].Value.Count - i] == false && _bufferInput[pCode].Used[_bufferInput[pCode].Value.Count - i] == true && _bufferInput[pCode].UsedInputs[_bufferInput[pCode].Value.Count - i].Contains(input) && !inputSequence.Hold)
            {
                return false;
            }

            if (inputSequence.Hold)
            {
                if (MatchHoldKeysForRelease(bufferedValue, moveValue))
                {
                    return false;
                }
            }

            //Check for a button press in the next buffer
            bool pressFound = false;
            for (int j = 0; j < bufferedValue.Count; j++)
            {
                if (bufferedValue[j].Style == InputStyle.Press)
                {
                    pressFound = true;
                    break;
                }
            }
            int combineOkayCount = 0;
            bool combineOkay = true;
            int cOct = 5;

            while (combineOkay && cOct > 0)
            {
                //If an input piece is matched
                if (MatchSimultaneousLastKeys(bufferedValue, moveValue))
                {
                    //Set the first found piece of the input to USED in the buffer list.
                    if (findCount == 1)
                    {
                        setUsed = _bufferInput[pCode].Value.Count - i;
                    }
                    //Cancel if the first input piece is found before the time limit.
                    if (findCount == 1 && _bufferInput[pCode].Buffer[_bufferInput[pCode].Value.Count - i] > BufferSubTimeOut)
                    {
                        findCount = 1;
                        setUsed = -1;
                        return false;
                    }
                    else
                    {
                        //Reset the input search if the next input wasn't placed in time.
                        if (findCount > 1 && (_bufferInput[pCode].Buffer[_bufferInput[pCode].Value.Count - i] - NextInputTime) > nextFoundTime)
                        {
                            findCount = 1;
                            setUsed = -1;
                            preciseSet = false;
                            combineOkay = false;
                        }
                        else
                        {
                            combineOkay = false;
                            for (int j = 0; j < moveValue.Count; j++)
                            {
                                if (moveValue[j].Combine)
                                {
                                    combineOkay = true;
                                }
                            }
                            if (findCount <= inputSequence.Value.Count)
                            {
                                preciseSet = inputSequence.Precise[inputSequence.Value.Count - findCount];
                            }
                            nextFoundTime = _bufferInput[pCode].Buffer[_bufferInput[pCode].Value.Count - i];
                            findCount++;

                            if (combineOkay)
                            {
                                combineOkayCount++;
                                moveValue = inputSequence.Value[inputSequence.Value.Count - findCount];
                            }
                        }
                    }
                }
                else
                {
                    combineOkay = false;
                    //Reset the search if the input was placed in the wrong order.
                    if (inputSequence.Tight[inputSequence.Value.Count - findCount] && preciseSet && pressFound && combineOkayCount <= 0)
                    {
                        findCount = 1;
                        setUsed = -1;
                        preciseSet = false;
                    }
                }
                cOct--;
            }
            

            //Confirm the input has been completely found once the increment meets the requirement.
            if (findCount > inputSequence.Value.Count)
            {
                if (!inputSequence.Hold)
                {
                    _bufferInput[pCode].Used[setUsed] = true;
                    _bufferInput[pCode].UsedInputs[setUsed].Add(input);
                    Debug.Log("Move " + input);
                }
                cancelCount = true;
                break;
            }
            
        }

        if (!cancelCount)
        {
            return false;
        }

        return true;
    }


    void Awake()
    {
        //There are four players allowed to make inputs. Separate the input buffer lists, etc. for these four.
        assignInputNames();
        assignInputReads();
        assignInputHoldCount();
        time = new System.TimeSpan[4];
        //timeSinceLastUpdate = new System.TimeSpan[4];
        timeSinceLastFullUpdate = System.TimeSpan.Zero;
        _lastBufferUpdateTime = new System.TimeSpan[4];
        _keys = new List<InputType>[4];
        _bufferInput = new BufferInput[4];
        //_bufferHold = new BufferHold[4];
        _mergeBuffer = new List<InputInfo>[4];
        _mergeInputTime = new System.TimeSpan[4];
        for (int i = 0; i < _keys.Length; i++)
        {
            _lastBufferUpdateTime[i] = System.TimeSpan.Zero;
            InputType[] inTy = System.Enum.GetValues(typeof(InputType)) as InputType[];
            _keys[i] = new List<InputType>();
            for (int j = 0; j < inTy.Length; j++)
            {
                _keys[i].Add(inTy[j]);
            }
            _bufferInput[i] = new BufferInput(new List<List<InputInfo>>(), new List<System.TimeSpan>());
            //_bufferHold[i] = new BufferHold(new List<InputInfo>(), new List<System.TimeSpan>());
            _mergeBuffer[i] = new List<InputInfo>();
            _mergeInputTime[i] = System.TimeSpan.Zero;
        }
    }

    void OnEnable()
    {
    #if UNITY_EDITOR
        EditorApplication.playmodeStateChanged += StateChange;
    #endif
    }

    #if UNITY_EDITOR
    void StateChange()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.isPlaying)
        {
            for (int pCode = 1; pCode <= 4; pCode++)
            {
                for (int i = 0; i < inputName.Count; i++)
                {
                    releasePlayerInput(inputName[i], pCode.ToString());
                }
            }
        }
    }
    #endif

    // Update is called once per frame
    void Update()
    {
        fullTime = System.TimeSpan.FromSeconds(Time.realtimeSinceStartup);
        for (int i = 0; i < 4; i++)
        {
            time[i] = System.TimeSpan.FromSeconds(Time.realtimeSinceStartup);
            //timeSinceLastUpdate[i] = time[i] - _lastBufferUpdateTime[i];

            var keysPressed = new List<InputInfo>();

            //Check if an input in the buffer list has expired.
            if (_bufferInput[i].Value.Count > 0)
            {
                for (int j = _bufferInput[i].Value.Count - 1; j >= 0; j--)
                {
                    _bufferInput[i].Buffer[j] += (fullTime - timeSinceLastFullUpdate);
                    if (_bufferInput[i].Buffer[j] > BufferTimeOut)
                    {
                        _bufferInput[i].Value.RemoveAt(j);
                        _bufferInput[i].Buffer.RemoveAt(j);
                        _bufferInput[i].Used.RemoveAt(j);
                        _bufferInput[i].UsedInputs.RemoveAt(j);
                    }
                }
            }

            //Check if an input in the buffer hold list has been released.
            /*if (_bufferHold[i].Value.Count > 0)
            {
                for (int j = _bufferHold[i].Value.Count - 1; j >= 0; j--)
                {
                    //Debug.Log(_bufferHold[i].Value[j].Value.ToString());
                    _bufferHold[i].Buffer[j] += (fullTime - timeSinceLastFullUpdate);
                    if (inputRelease[i].Count > 0)
                    {
                        for (int k = inputRelease[i].Count - 1; k >= 0; k--)
                        {
                            //Debug.Log(inputRelease[i][k].ipName);
                            if (inputRelease[i][k].ipName == _bufferHold[i].Value[j].Value.ToString())
                            {
                                _bufferHold[i].Value.RemoveAt(j);
                                _bufferHold[i].Buffer.RemoveAt(j);
                                inputRelease[i].RemoveAt(k);
                                break;
                            }
                        }
                    }
                }
            }*/

            /*if (i == 0)
            {
                if (_bufferHold[i].Value.Count > 0)
                {
                    Debug.Log(_bufferHold[i].ToString());
                }
            }*/

            //Check if an input in the buffer hold list has been released.
            for (int key = 0; key < _keys[i].Count; key++)
            {
                for (int j = 0; j < inputRelease[i].Count; j++)
                {
                    if (inputRelease[i][j].ipName == _keys[i][key].ToString())
                    {
                        keysPressed.Add(new InputInfo(_keys[i][key], InputStyle.Release, 0));
                        //inputHold[i].Remove(_keys[i][key].ToString());
                    }
                }
            }

            for (int k = inputRelease[i].Count - 1; k >= 0; k--)
            {
                inputRelease[i].RemoveAt(k);
            }

            // Get all of the keys pressed this frame.
            for (int key = 0; key < _keys[i].Count; key++)
            {
                for (int j = 0; j < inputRead[i].Count; j++)
                {
                    if (inputRead[i][j].ipName == _keys[i][key].ToString())
                    {
                        keysPressed.Add(new InputInfo(_keys[i][key], InputStyle.Press, 0));
                    }
                }
            }

            //Add any remaining inputs held to the buffer input list.
            for (int key = 0; key < _keys[i].Count; key++)
            {
                if (inputHold[i].ContainsKey(_keys[i][key].ToString()))
                {
                    keysPressed.Add(new InputInfo(_keys[i][key], InputStyle.Hold, inputHold[i][_keys[i][key].ToString()]));
                }
            }
            /*if (_bufferHold[i].Value.Count > 0)
            {
                for (int j = _bufferHold[i].Value.Count - 1; j >= 0; j--)
                {
                    //Debug.Log(_bufferHold[i].Value[j].Value.ToString());
                    _bufferHold[i].Buffer[j] += (fullTime - timeSinceLastFullUpdate);
                    if (inputRelease[i].Count > 0)
                    {
                        for (int k = inputRelease[i].Count - 1; k >= 0; k--)
                        {
                            //Debug.Log(inputRelease[i][k].ipName);
                            if (inputRelease[i][k].ipName == _bufferHold[i].Value[j].Value.ToString())
                            {
                                _bufferHold[i].Value.RemoveAt(j);
                                _bufferHold[i].Buffer.RemoveAt(j);
                                inputRelease[i].RemoveAt(k);
                                break;
                            }
                        }
                    }
                }
            }*/

            System.TimeSpan timeSinceMergeWindowOpen = time[i] - _mergeInputTime[i];
            // It is very hard to press two buttons on exactly the same frame.
            // If they are close enough, consider them pressed at the same time.
            bool isMergableInput = timeSinceMergeWindowOpen < MergeInputTime;

            if (isMergableInput)
            {
                _mergeBuffer[i].AddRange(keysPressed);
            }
            else
            {
                if (_mergeBuffer[i].Count > 0)
                {
                    //_buffer[i].Add(_mergeBuffer[i]);
                    _bufferInput[i].Value.Add(_mergeBuffer[i]);
                    _bufferInput[i].Buffer.Add(System.TimeSpan.Zero);
                    _bufferInput[i].Used.Add(false);
                    _bufferInput[i].UsedInputs.Add(new List<string>());
                    _lastBufferUpdateTime[i] = time[i];


                    /*int fCount = _mergeBuffer[i].Count;
                    for (int j = fCount - 1; j >= 0; j--)
                    {
                        for (int k = 0; k < _bufferHold[i].Value.Count; k++)
                        {
                            if (_mergeBuffer[i][j] == _bufferHold[i].Value[k])
                            {
                                _bufferHold[i].Value.Add(_mergeBuffer[i][j]);
                                _bufferHold[i].Buffer.Add(System.TimeSpan.Zero);
                                break;
                            }
                        }
                    }*/


                    // Clear the merge buffer
                    _mergeBuffer[i] = new List<InputInfo>();
                }

                // Start a new merge buffer
                if (keysPressed.Count > 0)
                {
                    _mergeBuffer[i] = keysPressed;
                    _mergeInputTime[i] = time[i];
                }
            }
        }

        runInputFrames((fullTime - timeSinceLastFullUpdate));

        timeSinceLastFullUpdate = fullTime;

        
    }

    /*void FixedUpdate()
    {
        
    }*/

    public void assignInputNames()
    {
        inputName = new List<string>();
        inputName.Add("Up");
        inputName.Add("Down");
        inputName.Add("Left");
        inputName.Add("Right");
        inputName.Add("Up_Left");
        inputName.Add("Up_Right");
        inputName.Add("Down_Left");
        inputName.Add("Down_Right");
        inputName.Add("Start");
        inputName.Add("Select");
        inputName.Add("A");
        inputName.Add("B");
        inputName.Add("C");
        inputName.Add("D");
        inputName.Add("L1");
        inputName.Add("R1");
        inputName.Add("L2");
        inputName.Add("R2");
        inputName.Add("L3");
        inputName.Add("R3");
        inputName.Add("R_Up");
        inputName.Add("R_Down");
        inputName.Add("R_Left");
        inputName.Add("R_Right");
        inputName.Add("R_Up_Left");
        inputName.Add("R_Up_Right");
        inputName.Add("R_Down_Left");
        inputName.Add("R_Down_Right");
        inputName.Add("K_Up");
        inputName.Add("K_Down");
        inputName.Add("K_Left");
        inputName.Add("K_Right");
        inputName.Add("K_Up_Left");
        inputName.Add("K_Up_Right");
        inputName.Add("K_Down_Left");
        inputName.Add("K_Down_Right");
    }

    public void assignInputReads()
    {
        inputRead = new List<InputPressData>[4];
        //_usedCode = new List<int>[4];
        for (int i = 0; i < inputRead.Length; i++)
        {
            inputRead[i] = new List<InputPressData>();

        }
        inputRelease = new List<InputPressData>[4];
        for (int i = 0; i < inputRelease.Length; i++)
        {
            inputRelease[i] = new List<InputPressData>();
        }
        inputHold = new Dictionary<string, System.TimeSpan>[4];
        for (int i = 0; i < inputHold.Length; i++)
        {
            inputHold[i] = new Dictionary<string, System.TimeSpan>();
        }
        axisKeyOn = new bool[4];
        for (int i = 0; i < inputHold.Length; i++)
        {
            axisKeyOn[i] = false;
        }
    }

    public void assignInputHoldCount()
    {
        inputHoldCount = new Dictionary<string, int>[4];
        for (int i = 0; i < inputHoldCount.Length; i++)
        {
            inputHoldCount[i] = new Dictionary<string, int>();
            inputHoldCount[i].Add("Up", 0);
            inputHoldCount[i].Add("Down", 0);
            inputHoldCount[i].Add("Left", 0);
            inputHoldCount[i].Add("Right", 0);
            inputHoldCount[i].Add("Up_Left", 0);
            inputHoldCount[i].Add("Up_Right", 0);
            inputHoldCount[i].Add("Down_Left", 0);
            inputHoldCount[i].Add("Down_Right", 0);
            inputHoldCount[i].Add("Start", 0);
            inputHoldCount[i].Add("Select", 0);
            inputHoldCount[i].Add("A", 0);
            inputHoldCount[i].Add("B", 0);
            inputHoldCount[i].Add("C", 0);
            inputHoldCount[i].Add("D", 0);
            inputHoldCount[i].Add("L1", 0);
            inputHoldCount[i].Add("R1", 0);
            inputHoldCount[i].Add("L2", 0);
            inputHoldCount[i].Add("R2", 0);
            inputHoldCount[i].Add("L3", 0);
            inputHoldCount[i].Add("R3", 0);
            inputHoldCount[i].Add("R_Up", 0);
            inputHoldCount[i].Add("R_Down", 0);
            inputHoldCount[i].Add("R_Left", 0);
            inputHoldCount[i].Add("R_Right", 0);
            inputHoldCount[i].Add("R_Up_Left", 0);
            inputHoldCount[i].Add("R_Up_Right", 0);
            inputHoldCount[i].Add("R_Down_Left", 0);
            inputHoldCount[i].Add("R_Down_Right", 0);
            inputHoldCount[i].Add("K_Up", 0);
            inputHoldCount[i].Add("K_Down", 0);
            inputHoldCount[i].Add("K_Left", 0);
            inputHoldCount[i].Add("K_Right", 0);
            inputHoldCount[i].Add("K_Up_Left", 0);
            inputHoldCount[i].Add("K_Up_Right", 0);
            inputHoldCount[i].Add("K_Down_Left", 0);
            inputHoldCount[i].Add("K_Down_Right", 0);
        }
    }

    //Check for inputs to add into the list of current buttons that are pressed.
    public void enterNewPlayerInput(string input, string playerCode)
    {
        try {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric && inpExists(input, inputName))
            {
                bool heldOn = false;
                foreach (KeyValuePair<string, System.TimeSpan> hld in inputHold[pCode - 1])
                {
                    if (input == hld.Key)
                    {
                        heldOn = true;
                        inputHoldCount[pCode - 1][hld.Key] += 1;
                    }
                }
                if (!heldOn)
                {
                    /*for (int i = 0; i < _keys[pCode - 1].Count; i++)
                    {
                        if (inputHoldCount[pCode - 1].ContainsKey(input))
                        {

                        }
                    }*/
                    inputRead[pCode - 1].Add(new InputPressData { ipName = input, ipDur = 0, ipRemove = false });
                    inputHold[pCode - 1].Add(input, System.TimeSpan.Zero);
                    //inputHoldCount[pCode - 1][input] += 1;
                    //Debug.Log("Added:" + inputRead[pCode - 1].Count);
                    //Debug.Log("Added:"+ inputHold[pCode - 1].Count);
                }
            }
        } catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Read is invalid");
        }
    }

    public void enterNewPlayerInput(string input, int playerCode)
    {
        try
        {
            if (inpExists(input, inputName))
            {
                bool heldOn = false;
                foreach (KeyValuePair<string, System.TimeSpan> hld in inputHold[playerCode])
                {
                    if (input == hld.Key)
                    {
                        Debug.Log(input + System.Convert.ToString(playerCode));
                        heldOn = true;
                        inputHoldCount[playerCode][hld.Key] += 1;
                    }
                }
                if (!heldOn)
                {
                    inputRead[playerCode].Add(new InputPressData { ipName = input, ipDur = 0, ipRemove = false });
                    inputHold[playerCode].Add(input, System.TimeSpan.Zero);
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Read is invalid");
        }
    }

    //Check for inputs to add into the list of current buttons that are released.
    public void releasePlayerInput(string input, string playerCode)
    {
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric && inpExists(input, inputName))
            {
                if (inputHold[pCode - 1].ContainsKey(input))
                {
                    bool heldOn = false;
                    string dec = "";
                    foreach (KeyValuePair<string, int> hld in inputHoldCount[pCode - 1])
                    {
                        if (input == hld.Key && inputHoldCount[pCode - 1][hld.Key] > 0)
                        {
                            heldOn = true;
                            dec = hld.Key;
                        }
                    }
                    if (heldOn)
                    {
                        inputHoldCount[pCode - 1][dec] -= 1;
                    }
                    if (!heldOn)
                    {
                        inputRelease[pCode - 1].Add(new InputPressData { ipName = input, ipDur = 0, ipRemove = false });
                        inputHold[pCode - 1].Remove(input);
                    }
                    //Debug.Log("Remove: "+ input + " : " + pCode);
                    /*
                    for (int j = 0; j < inputRead[pCode - 1].Count; j++)
                    {
                        Debug.Log(inputRead[pCode - 1][j].ipName);
                    }
                    for (int j = 0; j < inputRelease[pCode - 1].Count; j++)
                    {
                        Debug.Log(inputRelease[pCode - 1][j].ipName);
                    }*/
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Release is invalid");
        }
    }

    public void releasePlayerInput(string input, int playerCode)
    {
        try
        {
            if (inpExists(input, inputName))
            {
                if (inputHold[playerCode].ContainsKey(input))
                {
                    bool heldOn = false;
                    string dec = "";
                    foreach (KeyValuePair<string, int> hld in inputHoldCount[playerCode])
                    {
                        if (input == hld.Key && inputHoldCount[playerCode][hld.Key] > 0)
                        {
                            heldOn = true;
                            dec = hld.Key;
                        }
                    }
                    if (heldOn)
                    {
                        inputHoldCount[playerCode][dec] -= 1;
                    }
                    if (!heldOn)
                    {
                        inputRelease[playerCode].Add(new InputPressData { ipName = input, ipDur = 0, ipRemove = false });
                        inputHold[playerCode].Remove(input);
                    }
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Release is invalid");
        }
    }

    public void releaseAllHolds(int playerCode)
    {
        foreach (KeyValuePair<string, int> hld in inputHoldCount[playerCode])
        {
            if (inputHoldCount[playerCode][hld.Key] > 0)
            {
                inputHoldCount[playerCode][hld.Key] = 0;
            }
        }
        if (inputHold[playerCode].Count > 0)
        {
            foreach (KeyValuePair<string, System.TimeSpan> hld in inputHold[playerCode])
            {
                inputRelease[playerCode].Add(new InputPressData { ipName = hld.Key, ipDur = 0, ipRemove = false });
            }
        }
        inputHold[playerCode].Clear();
    }

    //Remove each found button once registered to the buffer list.
    public void runInputFrames(System.TimeSpan addTime)
    {
        for (int i = 0; i < inputRead.Length; i++)
        {
            for (int j = inputRead[i].Count - 1; j > -1; j--)
            {
                inputRead[i].RemoveAt(j);
            }
            /*for (int j = inputRelease[i].Count - 1; j > -1; j--)
            {
                if (inputRelease[i][j].ipRemove)
                {
                    Debug.Log("Removed:" + inputRelease[i][j].ipName);
                    inputRelease[i].RemoveAt(j);
                }
            }*/
            for (int j = 0; j < inputName.Count; j++)
            {
                if (inputHold[i].ContainsKey(inputName[j]))
                {
                    inputHold[i][inputName[j]] += addTime;
                }
            }
        }
    }

    //Execute the selected input command
    public bool useNewInput(string input, string playerCode)
    {
        bool found = false;
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            string[] _moveType = System.Enum.GetNames(typeof(MoveType));
            if (isNumeric && inpExists(input, _moveType))
            {
                if (MatchesLast(pCode - 1, UniversalMoveData.Moves[(MoveType)System.Enum.Parse(typeof(MoveType), input)], input))
                {
                    //inputRead[pCode - 1][j].ipRemove = true;
                    found = true;
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Use is invalid");
        }
        return found;
    }

    public bool useNewInput(string input, int playerCode)
    {
        bool found = false;
        try
        {
            string[] _moveType = System.Enum.GetNames(typeof(MoveType));
            if (inpExists(input, _moveType))
            {
                if (MatchesLast(playerCode, UniversalMoveData.Moves[(MoveType)System.Enum.Parse(typeof(MoveType), input)], input))
                {
                    found = true;
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Use is invalid");
        }
        return found;
    }

    //Execute the selected input command that requires holding
    //Note: May remove once input sequence is set to include holding states.
    public bool useHold(string input, string playerCode)
    {
        bool found = false;
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric && inpExists(input, inputName))
            {
                if (inputHold[pCode - 1].ContainsKey(input))
                {
                    found = true;
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Use is invalid");
        }
        return found;
    }

    public bool useHold(string input, int playerCode)
    {
        bool found = false;
        try
        {
            if (inpExists(input, inputName))
            {
                if (inputHold[playerCode].ContainsKey(input))
                {
                    found = true;
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Use is invalid");
        }
        return found;
    }

    //Execute the selected input command FROM ARROW KEYS that require holding
    //Note: May remove once input sequence is set to include holding states.
    public bool useKeyHold(string input, string playerCode)
    {
        bool found = false;
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric && inpExists(input, inputName))
            {
                if (inputHold[pCode - 1].ContainsKey(input))
                {
                    found = true;
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Use is invalid");
        }
        return found;
    }

    //Remove all use on inputs that require the axis if that axis is idle.
    public void undoDPad(string playerCode)
    {
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric)
            {
                for (int iPn = 0; iPn < inputName.Count; iPn++)
                {
                    if (inputHold[pCode - 1].ContainsKey(inputName[iPn]))
                    {
                        if (inputName[iPn] == "Up" || inputName[iPn] == "Up_Left" || inputName[iPn] == "Up_Right" || inputName[iPn] == "Down" || inputName[iPn] == "Down_Left" || inputName[iPn] == "Down_Right" || inputName[iPn] == "Left" || inputName[iPn] == "Right")
                        {
                            releasePlayerInput(inputName[iPn], playerCode);
                        }
                    }
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Use is invalid");
        }
    }

    //Check if the axis is not idle
    public bool checkReleased(string playerCode)
    {
        bool found = true;
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric)
            {
                if (MatchesLast(pCode - 1, UniversalMoveData.Moves[MoveType.MoveUp], "MoveUp") || MatchesLast(pCode - 1, UniversalMoveData.Moves[MoveType.MoveUp_Left], "MoveUp_Left") || MatchesLast(pCode - 1, UniversalMoveData.Moves[MoveType.MoveUp_Right], "MoveUp_Right") || MatchesLast(pCode - 1, UniversalMoveData.Moves[MoveType.MoveDown], "MoveDown") || MatchesLast(pCode - 1, UniversalMoveData.Moves[MoveType.MoveDown_Left], "MoveDown_Left") || MatchesLast(pCode - 1, UniversalMoveData.Moves[MoveType.MoveDown_Right], "MoveDown_Right") || MatchesLast(pCode - 1, UniversalMoveData.Moves[MoveType.MoveLeft], "MoveLeft") || MatchesLast(pCode - 1, UniversalMoveData.Moves[MoveType.MoveRight], "MoveRight"))
                {
                    found = false;
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Release Check is invalid");
        }
        return found;
    }

    public bool checkReleased(int playerCode)
    {
        bool found = true;
        try
        {
            if (MatchesLast(playerCode, UniversalMoveData.Moves[MoveType.MoveUp], "MoveUp") || MatchesLast(playerCode, UniversalMoveData.Moves[MoveType.MoveUp_Left], "MoveUp_Left") || MatchesLast(playerCode, UniversalMoveData.Moves[MoveType.MoveUp_Right], "MoveUp_Right") || MatchesLast(playerCode, UniversalMoveData.Moves[MoveType.MoveDown], "MoveDown") || MatchesLast(playerCode, UniversalMoveData.Moves[MoveType.MoveDown_Left], "MoveDown_Left") || MatchesLast(playerCode, UniversalMoveData.Moves[MoveType.MoveDown_Right], "MoveDown_Right") || MatchesLast(playerCode, UniversalMoveData.Moves[MoveType.MoveLeft], "MoveLeft") || MatchesLast(playerCode, UniversalMoveData.Moves[MoveType.MoveRight], "MoveRight"))
            {
                found = false;
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Release Check is invalid");
        }
        return found;
    }

    //Check is any ARROW KEYS are not held
    public bool checkKeyReleased(string playerCode)
    {
        bool found = true;
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric)
            {
                for (int iPn = 0; iPn < inputName.Count; iPn++)
                {
                    if (inputHold[pCode - 1].ContainsKey(inputName[iPn]))
                    {
                        if (inputName[iPn] == "K_Up" || inputName[iPn] == "K_Up_Left" || inputName[iPn] == "K_Up_Right" || inputName[iPn] == "K_Down" || inputName[iPn] == "K_Down_Left" || inputName[iPn] == "K_Down_Right" || inputName[iPn] == "K_Left" || inputName[iPn] == "K_Right")
                        {
                            found = false;
                        }
                    }
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Release Check is invalid");
        }
        return found;
    }

    //Set the boolean for when ARROW KEYS are used.
    public void turnAxisKey(string playerCode, bool state)
    {
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric)
            {
                axisKeyOn[pCode - 1] = state;
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Axis Key is invalid");
        }
    }

    //Check if ARROW KEYS are used.
    public bool axisKeyIsOn(string playerCode)
    {
        bool state = false;
        try
        {
            int pCode;
            bool isNumeric = int.TryParse(playerCode, out pCode);
            if (isNumeric)
            {
                state = axisKeyOn[pCode - 1];
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Axis Key is invalid");
        }
        return state;
    }

    //Check if the name of the input command exists
    private bool inpExists(string input, List<string> inputNme)
    {
        for (int i = 0; i < inputNme.Count; i++)
        {
            if (input == inputNme[i])
            {
                return true;
            }
        }
        return false;
    }

    //Check if the name of the input command exists
    private bool inpExists(string input, string[] inputNme)
    {
        for (int i = 0; i < inputNme.Length; i++)
        {
            if (input == inputNme[i].ToString())
            {
                return true;
            }
        }
        return false;
    }
}
