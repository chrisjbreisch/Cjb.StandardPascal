program TheBoss (input, output); 
    {Demonstrates simple input and output.} 

var Year, ChartPosition: integer; 

begin 
    writeln ('When was Born to Run released? How high did it go?'); 
    readln (Year, ChartPosition); 
    writeln ('Born to Run came out in', Year,' and hit #', ChartPosition); 
    writeln (Year, ChartPosition) 
end. {TheBoss} 