program AlphabetSoup (output); 
    {Uses for loops to print the alphabet forward and backward.} 

var CounterCharacter: char; 

begin 
    for CounterCharacter := 'A' to 'Z' do 
        write (CounterCharacter); 

    writeln;     

    for CounterCharacter := 'z' downto 'a' do 
        write (CounterCharacter); 

    writeln 
end.