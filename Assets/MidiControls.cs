using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//you need to declare this!
using MidiJack;

//uses https://github.com/keijiro/MidiJack
//to test the connection open the menu Window -> MIDI Jack to see the midi signals

//Note: on Windows MidiJack works in editor but
//may not work properly when the application is built
//the MIDI input is frozen for a few minutes after the app or unity is closed
//

public class MidiControls : MonoBehaviour
{
    public GameObject sphere;

    //on AKAI mpk 70 71 are the first two knobs
    //on Korg nanocontrol the sliders go from 0 to 8
    public int testKnob1 = 70;
    public int testKnob2 = 71;

    //used for number->note conversion
    private string[] noteString = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    //pupulated at the beginning based on the available CC Continuous Controllers: knobs, sliders, etc
    public int[] controllerNumbers;

    void OnEnable()
    {
        MidiMaster.noteOnDelegate += OnNoteOn;
        MidiMaster.noteOffDelegate += OnNoteOff;
        MidiMaster.knobDelegate += OnControllerChange;

    }

    




    // Update is called once per frame
    void Update()
    {
        //48 = C on 3 octave
        //important: on the akai keyboard there are oct- and oct+ buttons that will affect this

        float vel = MidiMaster.GetKey(48);

        //velocity is the intensity of the key press (loudness)
        if (vel>0)
        {
            print("C "+vel);
        }

        //only detects if when a key goes down, GetKeyUp is the same
        if (MidiMaster.GetKeyDown(50))
        {
            print("D played");
        }


        //the controllers will be visible only the first time they are used
        controllerNumbers = MidiMaster.GetKnobNumbers();

        //check the controllers values
        for (int i=0; i< controllerNumbers.Length; i++)
        {
            //the number identifying the CC
            int CCnum = controllerNumbers[i];

            float value = MidiMaster.GetKnob(CCnum);
            print("CC "+CCnum+": "+value);

        }



        //testing inputs
        //0 is optional and it's the default value if it hasn't been touched yet 
        float vx = MidiMaster.GetKnob(testKnob1, 0);
        float vy = MidiMaster.GetKnob(testKnob2, 0);

        sphere.transform.localScale = new Vector3(1+vx, 1+vy, 1+vx);

        //48 = C3 on keyboard
        float v1 = MidiMaster.GetKey(48);

        if (v1 > 0)
        {
            sphere.transform.Translate(-Time.deltaTime* v1, 0,0);
        }

        //50 = D3 on keyboard
        float v2 = MidiMaster.GetKey(50);

        if (v2 > 0)
        {
            sphere.transform.Translate(Time.deltaTime* v2, 0, 0);
        }


    }


    //delegate functions, detect the same input but as events within functions
    //ie when values change instead of continuous on the update function

    //called whenever a note is on (key pressed)
    void OnNoteOn(MidiChannel channel, int note, float velocity)
    {
        print("Note on: MIDI number " + note + " Note: " + GetNote(note) + " Octave: " + GetOctave(note)+ " velocity "+velocity);
    }

    //called whenever a note is off (key released)
    void OnNoteOff(MidiChannel channel, int note)
    {
        print("Note on: MIDI number " + note + " Note: " + GetNote(note) + " Octave: " + GetOctave(note));
    }

    //called whenever a CC changes value
    void OnControllerChange(MidiChannel channel, int number, float value)
    {
        print("CC change: number " + number + " Value: " + value);
        
    }


    //conversion functions for the keyboard: given the number get the note and octave
    string GetNote(int midiNumber)
    {
        int octave = (midiNumber / 12) - 1;
        int noteIndex = (midiNumber % 12);
        string note = noteString[noteIndex];
        return note;
    }

    int GetOctave(int midiNumber)
    {
        int octave = (midiNumber / 12) - 1;
        int noteIndex = (midiNumber % 12);
        string note = noteString[noteIndex];
        return octave;
    }

   
    void OnDisable()
    {
        MidiMaster.noteOnDelegate -= OnNoteOn;
        MidiMaster.noteOffDelegate -= OnNoteOff;
        MidiMaster.knobDelegate -= OnControllerChange;
    }
}
