using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVoiceActivityDetection
{
    public event Action<bool> OnVoiceDetected;              
    public event Action OnVoiceLost;                        
    public event Action OnVoiceEnded;                        
    public event Action OnTimeUpdate;                       
    public event Action OnWaitEnded;
    
    public void ResponseReceived(bool value);
    public void ResponseEnded();
    public void ResponseExpected(int maxRecordTime);
}
