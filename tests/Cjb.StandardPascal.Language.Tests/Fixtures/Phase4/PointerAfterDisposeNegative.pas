program PointerAfterDisposeNegative;
type
  IntPointer = ^integer;
var
  pointer: IntPointer;
begin
  new(pointer);
  dispose(pointer);
  writeln(pointer^);
end.