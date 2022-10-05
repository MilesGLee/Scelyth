using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager
{
    private KeyCode _mobility = KeyCode.Space;
    private KeyCode _focus = KeyCode.Mouse2;

    public KeyCode Mobility { get { return _mobility; } }
    public KeyCode Focus { get { return _focus; } }
}
