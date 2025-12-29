#include <vector>

#ifndef GRID
#define GRID
class Grid
{
public:
	enum stoneType
	{
		none = 0,
		white = 1,
		black = 2
	};
private:
	stoneType type;
	int x;
	int y;
	std::vector<Grid> connectedStones;

public:
	Grid();
	Grid(stoneType stone, int x, int y);
	int xPos() const{ return x; }// マスのx座標を返す
	int yPos() const{ return y; }// マスのy座標を返す
	stoneType GetStoneType() const { return type; }// マスに置かれている石の色を返す
	std::vector<Grid> GetConnectedStones() const { return connectedStones; }// マスに置かれている石と同じ色（同時にひっくり返せる）のマスの一覧を返す
	void SetStoneType(stoneType type);
	void SetConnectedStones(Grid stone);
	void SetConnectedStones(std::vector<Grid> stones);
};
#endif // !GRID

