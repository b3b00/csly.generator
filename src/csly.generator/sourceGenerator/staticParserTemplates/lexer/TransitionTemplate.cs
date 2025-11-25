
if (<#CONDITION#>)
{

    _currentState = <#NEW_STATE#>;
    _currentPosition.Index++;
    <#ENDING#>
    return (true, _currentState, _currentMatch);

}