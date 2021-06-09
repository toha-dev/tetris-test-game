using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDownWrapper
{
    private KeyCode _key;

    private bool _isKeyDown = false;
    private bool _isKeyDownStateRequested = false;

    public KeyDownWrapper(KeyCode key) => _key = key;

    public bool IsKeyDown()
    {
        if (Input.GetKey(_key))
            _isKeyDown = true;
        else
        {
            _isKeyDown = false;
            _isKeyDownStateRequested = false;
        }
        
        if (_isKeyDownStateRequested)
            return false;

        if (!_isKeyDown)
            return false;

        _isKeyDownStateRequested = true;
        return _isKeyDown;
    }
}
