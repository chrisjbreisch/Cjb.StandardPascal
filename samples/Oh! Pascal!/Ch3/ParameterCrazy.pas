program ParameterCrazy (input, output); 
    {Reverses two input integers.} 

var First, Second: integer; 

procedure GetTheNumbers (var First, Second: integer); 
    {Reads values for the variable parameters First and Second.} 

    begin 
        writeln ('This program reverses two integers.'); 
        writeln ('What is the first number?'); 
        readln (First); 
        writeln ('What is the second number?'); 
        readln (Second); 
    end; {GetTheNumbers} 

procedure SwitchThem (var First, Second: integer); 
    {Swaps the values of two variable parameters.} 

    var Temporary: integer;

    begin 
        Temporary := First; 
        First := Second; 
        Second := Temporary 
    end; {SwitchThem} 

procedure PrintTheResults (First, Second: integer); 
    {Prints its value parameters.} 

    const PRINTSPACE = 1 ; {Provide the minimum output field.} 

    begin 
        write ('In reversed order, the numbers are '); 
        writeln (First:PRINTSPACE,' and ', Second:PRINTSPACE,'.') 
    end; {PrintTheResults} 

begin {The main program, ParameterCrazy} 
    GetTheNumbers (First, Second);
    SwitchThem (First, Second);
    PrintTheResults (First, Second) 
end. {ParameterCrazy}