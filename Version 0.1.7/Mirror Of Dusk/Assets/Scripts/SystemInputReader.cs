using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SystemInputReader : MonoBehaviour {

    public List<string> inputName;                  //List of available input buttons
    public List<InputPressData> inputRead;        //List of current buttons "PRESSED" that are in use
    public List<InputPressData> inputRelease;     //List of current buttons "RELEASED" that are to be confirmed
    public Dictionary<string, System.TimeSpan> inputHold;     //List of current buttons "HELD" that are in use
    //public Dictionary<string, int> inputHoldCount;            //List of current buttons "HELD" that are pressed by more than one of the same key
    public class InputPressData
    {
        public string ipName;           //Name of input
        public int ipDur = 0;           //*DISCONTINUED!!*  Duration for input to be read
        public bool ipRemove = false;   //*DISCONTINUED!!*  Trigger for input to be removed
    }

    private System.TimeSpan time;                     //Current time for each player
    private System.TimeSpan fullTime;                   //Current overall time
    private System.TimeSpan timeSinceLastFullUpdate;

    private static readonly System.TimeSpan BufferTimeOut = System.TimeSpan.FromMilliseconds(1000);     //Time before an input buffer is cleared
    private static readonly System.TimeSpan BufferSubTimeOut = System.TimeSpan.FromMilliseconds(400);   //Time before an input buffer is disabled from use
    private static readonly System.TimeSpan MergeInputTime = System.TimeSpan.FromMilliseconds(20);      //Time before all current inputs are merged into a simultaneous press
    private static readonly System.TimeSpan NextInputTime = System.TimeSpan.FromMilliseconds(250);      //Time before the next required input can be met

    //private System.TimeSpan _lastBufferUpdateTime;
    private System.TimeSpan _mergeInputTime;

    private BufferInput _bufferInput;         //List of current inputs in buffer order.
    private List<InputInfo> _mergeBuffer;     //List of current inputs ready to merge.

    private List<InputType> _keys;            //List of available keys to add before next merge.

    //Enum list for types of move command labels
    public enum MoveType
    {
        MoveRight, MoveLeft, MoveUp, MoveDown, MoveUp_Right, MoveUp_Left, MoveDown_Right, MoveDown_Left, Confirm, Cancel, Pause
    }

    //Enum list for types of available input buttons. Note:R_ is for Right Analog Stick. K_ is for Keyboard Arrows.
    public enum InputType
    {
        UI_Up, UI_Down, UI_Left, UI_Right, UI_UpLeft, UI_UpRight, UI_DownLeft, UI_DownRight,
        Confirm, Cancel, UI_Pause
    }

    //Enum list for types of available input commands that can halt a command if wrongly pressed.
    public enum InputSkip
    {
        Confirm, Cancel, UI_Pause
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

    //Available list of commands
    public static class SystemMoveData
    {
        public static readonly Dictionary<MoveType, InputSequence> Moves = new Dictionary<MoveType, InputSequence>();

        static SystemMoveData()
        {
            Moves.Add(MoveType.Pause, new InputSequence(new[,] { { new InputInfo(InputType.UI_Pause, InputStyle.Press, 0) } }, false, false));
            Moves.Add(MoveType.Cancel, new InputSequence(new[,] { { new InputInfo(InputType.Cancel, InputStyle.Press, 0) } }, false, false));
            Moves.Add(MoveType.Confirm, new InputSequence(new[,] { { new InputInfo(InputType.Confirm, InputStyle.Press, 0) } }, false, false));
            Moves.Add(MoveType.MoveDown_Left, new InputSequence(new[,] { { new InputInfo(InputType.UI_DownLeft, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveDown_Right, new InputSequence(new[,] { { new InputInfo(InputType.UI_DownRight, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveUp_Left, new InputSequence(new[,] { { new InputInfo(InputType.UI_UpLeft, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveUp_Right, new InputSequence(new[,] { { new InputInfo(InputType.UI_UpRight, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveDown, new InputSequence(new[,] { { new InputInfo(InputType.UI_Down, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveUp, new InputSequence(new[,] { { new InputInfo(InputType.UI_Up, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveLeft, new InputSequence(new[,] { { new InputInfo(InputType.UI_Left, InputStyle.Hold, 0) } }, true, false));
            Moves.Add(MoveType.MoveRight, new InputSequence(new[,] { { new InputInfo(InputType.UI_Right, InputStyle.Hold, 0) } }, true, false));
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
    public bool MatchesLast(InputSequence inputSequence, string input)
    {
        // If the move is longer than the buffer, it can't possibly match.
        if (_bufferInput.Value.Count < inputSequence.Value.Count)
        {
            return false;
        }

        int setUsed = -1;           //Trace the array key of any input that is set to be labelled as USED.
        int findCount = 1;          //Increment for each piece of the input command that is found in order.
        bool preciseSet = false;    //For confirming if the input restricts wrong follow-ups.
        bool cancelCount = false;   //Stop the search for the remaining inputs in the buffer list once a match is completely found.
        System.TimeSpan nextFoundTime;      //The time remaining for the next input to be followed up.

        // Loop backwards to match against the most recent input.
        for (int i = 1; i <= _bufferInput.Value.Count; ++i)
        {
            var bufferedValue = _bufferInput.Value[_bufferInput.Value.Count - i];     //Value of current input in buffer list.
            var moveValue = inputSequence.Value[inputSequence.Value.Count - findCount];             //Value of current input in the required input command sequence.

            //If the next first input is found before the last used input in the buffer list, cancel the move.
            if (findCount <= 1 && _bufferInput.Used[_bufferInput.Value.Count - i] == true && !inputSequence.Hold)
            {
                return false;
            }

            if (_bufferInput.Used[_bufferInput.Value.Count - i] == false && _bufferInput.Used[_bufferInput.Value.Count - i] == true && _bufferInput.UsedInputs[_bufferInput.Value.Count - i].Contains(input) && !inputSequence.Hold)
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
                        setUsed = _bufferInput.Value.Count - i;
                    }
                    //Cancel if the first input piece is found before the time limit.
                    if (findCount == 1 && _bufferInput.Buffer[_bufferInput.Value.Count - i] > BufferSubTimeOut)
                    {
                        findCount = 1;
                        setUsed = -1;
                        return false;
                    }
                    else
                    {
                        //Reset the input search if the next input wasn't placed in time.
                        if (findCount > 1 && (_bufferInput.Buffer[_bufferInput.Value.Count - i] - NextInputTime) > nextFoundTime)
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
                            nextFoundTime = _bufferInput.Buffer[_bufferInput.Value.Count - i];
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
                    _bufferInput.Used[setUsed] = true;
                    _bufferInput.UsedInputs[setUsed].Add(input);
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
        //assignInputHoldCount();
        time = new System.TimeSpan();
        timeSinceLastFullUpdate = System.TimeSpan.Zero;

        //_lastBufferUpdateTime = System.TimeSpan.Zero;
        InputType[] inTy = System.Enum.GetValues(typeof(InputType)) as InputType[];
        _keys = new List<InputType>();
        for (int j = 0; j < inTy.Length; j++)
        {
            _keys.Add(inTy[j]);
        }
        _bufferInput = new BufferInput(new List<List<InputInfo>>(), new List<System.TimeSpan>());
        _mergeBuffer = new List<InputInfo>();
        _mergeInputTime = System.TimeSpan.Zero;
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
            for (int i = 0; i < inputName.Count; i++)
            {
                releasePlayerInput(inputName[i]);
            }
        }
    }
    #endif

    // Update is called once per frame
    void Update()
    {
        fullTime = System.TimeSpan.FromSeconds(Time.realtimeSinceStartup);
        time = System.TimeSpan.FromSeconds(Time.realtimeSinceStartup);

        var keysPressed = new List<InputInfo>();

        //Check if an input in the buffer list has expired.
        if (_bufferInput.Value.Count > 0)
        {
            for (int j = _bufferInput.Value.Count - 1; j >= 0; j--)
            {
                _bufferInput.Buffer[j] += (fullTime - timeSinceLastFullUpdate);
                if (_bufferInput.Buffer[j] > BufferTimeOut)
                {
                    _bufferInput.Value.RemoveAt(j);
                    _bufferInput.Buffer.RemoveAt(j);
                    _bufferInput.Used.RemoveAt(j);
                    _bufferInput.UsedInputs.RemoveAt(j);
                }
            }
        }

        //Check if an input in the buffer hold list has been released.
        for (int key = 0; key < _keys.Count; key++)
        {
            for (int j = 0; j < inputRelease.Count; j++)
            {
                if (inputRelease[j].ipName == _keys[key].ToString())
                {
                    keysPressed.Add(new InputInfo(_keys[key], InputStyle.Release, 0));
                    //inputHold.Remove(_keys[key].ToString());
                }
            }
        }

        for (int k = inputRelease.Count - 1; k >= 0; k--)
        {
            inputRelease.RemoveAt(k);
        }

        // Get all of the keys pressed this frame.
        for (int key = 0; key < _keys.Count; key++)
        {
            for (int j = 0; j < inputRead.Count; j++)
            {
                if (inputRead[j].ipName == _keys[key].ToString())
                {
                    keysPressed.Add(new InputInfo(_keys[key], InputStyle.Press, 0));
                }
            }
        }

        //Add any remaining inputs held to the buffer input list.
        for (int key = 0; key < _keys.Count; key++)
        {
            if (inputHold.ContainsKey(_keys[key].ToString()))
            {
                keysPressed.Add(new InputInfo(_keys[key], InputStyle.Hold, inputHold[_keys[key].ToString()]));
            }
        }

        System.TimeSpan timeSinceMergeWindowOpen = time - _mergeInputTime;
        // It is very hard to press two buttons on exactly the same frame.
        // If they are close enough, consider them pressed at the same time.
        bool isMergableInput = timeSinceMergeWindowOpen < MergeInputTime;

        if (isMergableInput)
        {
            _mergeBuffer.AddRange(keysPressed);
        }
        else
        {
            if (_mergeBuffer.Count > 0)
            {
                //_buffer.Add(_mergeBuffer[i]);
                _bufferInput.Value.Add(_mergeBuffer);
                _bufferInput.Buffer.Add(System.TimeSpan.Zero);
                _bufferInput.Used.Add(false);
                _bufferInput.UsedInputs.Add(new List<string>());
                //_lastBufferUpdateTime = time;


                // Clear the merge buffer
                _mergeBuffer = new List<InputInfo>();
            }

            // Start a new merge buffer
            if (keysPressed.Count > 0)
            {
                _mergeBuffer = keysPressed;
                _mergeInputTime = time;
            }
        }

        runInputFrames((fullTime - timeSinceLastFullUpdate));

        timeSinceLastFullUpdate = fullTime;


    }

    public void assignInputNames()
    {
        inputName = new List<string>();
        inputName.Add("UI_Up");
        inputName.Add("UI_Down");
        inputName.Add("UI_Left");
        inputName.Add("UI_Right");
        inputName.Add("UI_UpLeft");
        inputName.Add("UI_UpRight");
        inputName.Add("UI_DownLeft");
        inputName.Add("UI_DownRight");
        inputName.Add("Confirm");
        inputName.Add("Cancel");
        inputName.Add("UI_Pause");
    }

    public void assignInputReads()
    {
        inputRead = new List<InputPressData>();
        inputRelease = new List<InputPressData>();
        inputHold = new Dictionary<string, System.TimeSpan>();
    }

    /*public void assignInputHoldCount()
    {
        inputHoldCount = new Dictionary<string, int>();
        inputHoldCount.Add("UI_Up", 0);
        inputHoldCount.Add("UI_Down", 0);
        inputHoldCount.Add("UI_Left", 0);
        inputHoldCount.Add("UI_Right", 0);
        inputHoldCount.Add("UI_UpLeft", 0);
        inputHoldCount.Add("UI_UpRight", 0);
        inputHoldCount.Add("UI_DownLeft", 0);
        inputHoldCount.Add("UI_DownRight", 0);
        inputHoldCount.Add("Confirm", 0);
        inputHoldCount.Add("Cancel", 0);
        inputHoldCount.Add("UI_Pause", 0);
    }*/

    //Check for inputs to add into the list of current buttons that are pressed.
    public void enterNewPlayerInput(string input)
    {
        try
        {
            if (inpExists(input, inputName))
            {
                /*bool heldOn = false;
                foreach (KeyValuePair<string, System.TimeSpan> hld in inputHold)
                {
                    if (input == hld.Key)
                    {
                        heldOn = true;
                        //inputHoldCount[hld.Key] += 1;
                    }
                }
                if (!heldOn)
                {*/
                    if (!inputHold.ContainsKey(input))
                    {
                        inputRead.Add(new InputPressData { ipName = input, ipDur = 0, ipRemove = false });
                        inputHold.Add(input, System.TimeSpan.Zero);
                    }
                //}
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Read is invalid");
        }
    }

    //Check for inputs to add into the list of current buttons that are released.
    public void releasePlayerInput(string input)
    {
        try
        {
            if (inpExists(input, inputName))
            {
                if (inputHold.ContainsKey(input))
                {
                    //bool heldOn = false;
                    /*string dec = "";
                    foreach (KeyValuePair<string, int> hld in inputHoldCount)
                    {
                        if (input == hld.Key && inputHoldCount[hld.Key] > 0)
                        {
                            heldOn = true;
                            dec = hld.Key;
                        }
                    }
                    if (heldOn)
                    {
                        inputHoldCount[dec] -= 1;
                    }*/
                    //if (!heldOn)
                    //{
                        inputRelease.Add(new InputPressData { ipName = input, ipDur = 0, ipRemove = false });
                        inputHold.Remove(input);
                    //}
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Release is invalid");
        }
    }

    public void releaseAllHolds()
    {
        /*foreach (KeyValuePair<string, int> hld in inputHoldCount)
        {
            if (inputHoldCount[hld.Key] > 0)
            {
                inputHoldCount[hld.Key] = 0;
            }
        }*/
        if (inputHold.Count > 0)
        {
            foreach (KeyValuePair<string, System.TimeSpan> hld in inputHold)
            {
                inputRelease.Add(new InputPressData { ipName = hld.Key, ipDur = 0, ipRemove = false });
            }
        }
        inputHold.Clear();
    }

    //Remove each found button once registered to the buffer list.
    public void runInputFrames(System.TimeSpan addTime)
    {
        for (int j = inputRead.Count - 1; j > -1; j--)
        {
            inputRead.RemoveAt(j);
        }
        for (int j = 0; j < inputName.Count; j++)
        {
            if (inputHold.ContainsKey(inputName[j]))
            {
                inputHold[inputName[j]] += addTime;
            }
        }
    }

    //Execute the selected input command
    public bool useNewInput(string input)
    {
        bool found = false;
        try
        {
            string[] _moveType = System.Enum.GetNames(typeof(MoveType));
            if (inpExists(input, _moveType))
            {
                if (MatchesLast(SystemMoveData.Moves[(MoveType)System.Enum.Parse(typeof(MoveType), input)], input))
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
    public bool useHold(string input)
    {
        bool found = false;
        try
        {
            if (inpExists(input, inputName))
            {
                if (inputHold.ContainsKey(input))
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
    /*public void undoDPad()
    {
        try
        {
            for (int iPn = 0; iPn < inputName.Count; iPn++)
            {
                if (inputHold.ContainsKey(inputName[iPn]))
                {
                    if (inputName[iPn] == "UI_Up" || inputName[iPn] == "UI_UpLeft" || inputName[iPn] == "UI_UpRight" || inputName[iPn] == "UI_Down" || inputName[iPn] == "UI_DownLeft" || inputName[iPn] == "UI_DownRight" || inputName[iPn] == "UI_Left" || inputName[iPn] == "UI_Right")
                    {
                        releasePlayerInput(inputName[iPn]);
                    }
                }
            }
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("Error: Input Use is invalid");
        }
    }*/

    //Check if the axis is not idle
    public bool checkReleased()
    {
        bool found = true;
        try
        {
            if (MatchesLast(SystemMoveData.Moves[MoveType.MoveUp], "MoveUp") || MatchesLast(SystemMoveData.Moves[MoveType.MoveUp_Left], "MoveUp_Left") || MatchesLast(SystemMoveData.Moves[MoveType.MoveUp_Right], "MoveUp_Right") || MatchesLast(SystemMoveData.Moves[MoveType.MoveDown], "MoveDown") || MatchesLast(SystemMoveData.Moves[MoveType.MoveDown_Left], "MoveDown_Left") || MatchesLast(SystemMoveData.Moves[MoveType.MoveDown_Right], "MoveDown_Right") || MatchesLast(SystemMoveData.Moves[MoveType.MoveLeft], "MoveLeft") || MatchesLast(SystemMoveData.Moves[MoveType.MoveRight], "MoveRight"))
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
