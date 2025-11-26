
if (<#CONDITION#>)
{

    _currentState = <#NEW_STATE#>;
    _currentPosition.Index++;
    _currentPosition.Column++;
    <#ENDING#>
    return (true, _currentState, _currentMatch);

}