#include "Board.h"
#include <iostream>

void CheckIfGridNone(int num, Grid grid, Grid tempGrid, std::vector<Grid>& connectedStones);
bool StringToInt(std::string string, int& num, int maxDigits);

Board::Board()
{
	this->width = 1;
	this->height = 1;
	this->WhiteNum = 0;
	this->BlackNum = 0;

	for (int h = 0; h < GetHeight(); h++)
	{
		for (int w = 0; w < GetWidth(); w++)
		{
			grids[h].emplace_back(Grid(Grid::none, w, h));
		}
	}
}

Board::Board(int width, int height)
{
	this->width = width;
	this->height = height;
	this->WhiteNum = 0;
	this->BlackNum = 0;

	ResetBoard();
}

void Board::SetWhiteNum(int Num)
{
	WhiteNum = Num;
}
void Board::SetBlackNum(int Num)
{
	BlackNum = Num;
}

bool Board::TryGetGrid(Grid grid, direction dir, Grid& outputGrid)
{
	int x = grid.xPos() - 1;
	int y = grid.yPos() - 1;
	switch (dir)
	{
	case Board::up:
		if (grid.yPos() - 1 <= 0)
		{
			return false;
		}

		outputGrid = grids[y - 1][x];
		return true;
		break;
	case Board::down:
		if (grid.yPos() + 1 > GetHeight())
		{
			return false;
		}

		outputGrid = grids[y + 1][x];
		return true;
		break;
	case Board::left:
		if (grid.xPos() - 1 <= 0)
		{
			return false;
		}

		outputGrid = grids[y][x - 1];
		return true;
		break;
	case Board::right:
		if (grid.xPos() + 1 > GetWidth())
		{
			return false;
		}

		outputGrid = grids[y][x + 1];
		return true;
		break;
	case Board::upperLeft:
		if (grid.yPos() - 1 <= 0 || grid.xPos() - 1 <= 0)
		{
			return false;
		}

		outputGrid = grids[y - 1][x - 1];
		return true;
		break;
	case Board::upperRight:
		if (grid.yPos() - 1 <= 0 || grid.xPos() + 1 > GetWidth())
		{
			return false;
		}

		outputGrid = grids[y - 1][x + 1];
		return true;
		break;
	case Board::lowerLeft:
		if (grid.yPos() + 1 > GetHeight() || grid.xPos() - 1 <= 0)
		{
			return false;
		}

		outputGrid = grids[y + 1][x - 1];
		return true;
		break;
	case Board::lowerRight:
		if (grid.yPos() + 1 > GetHeight() || grid.xPos() + 1 > GetWidth())
		{
			return false;
		}

		outputGrid = grids[y + 1][x + 1];
		return true;
		break;
	default:
		return false;
		break;
	}
}

std::vector<Grid> const Board::SearchFlippableGrids(Grid grid, direction dir, Grid::stoneType type)// 指定した方向のひっくり返せるマスを調べる
{
	std::vector<Grid> connectedStones;
	Grid tempGrid;
	Grid::stoneType enemyType = type == Grid::white ? Grid::black : Grid::white;
	switch (dir)
	{
	case up:
		for (int j = 0; j < grid.yPos(); j++)
		{
			Grid gridBase = Grid(grid.GetStoneType(), grid.xPos(), grid.yPos() - j);
			if (!TryGetGrid(gridBase, up, tempGrid))
			{
				connectedStones.clear();
				break;
			}
			if (tempGrid.GetStoneType() != enemyType)
			{
				CheckIfGridNone(j, grid, tempGrid, connectedStones);
				break;
			}
			connectedStones.emplace_back(tempGrid);
		}
		break;
	case down:
		for (int j = 0; j < GetHeight() - grid.yPos() + 1; j++)
		{
			Grid gridBase = Grid(grid.GetStoneType(), grid.xPos(), grid.yPos() + j);
			if (!TryGetGrid(gridBase, down, tempGrid))
			{
				connectedStones.clear();
				break;
			}
			if (tempGrid.GetStoneType() != enemyType)
			{
				CheckIfGridNone(j, grid, tempGrid, connectedStones);
				break;
			}
			connectedStones.emplace_back(tempGrid);
		}
		break;
	case left:
		for (int j = 0; j < grid.xPos(); j++)
		{
			Grid gridBase = Grid(grid.GetStoneType(), grid.xPos() - j, grid.yPos());
			if (!TryGetGrid(gridBase, left, tempGrid))
			{
				connectedStones.clear();
				break;
			}
			if (tempGrid.GetStoneType() != enemyType)
			{
				CheckIfGridNone(j, grid, tempGrid, connectedStones);
				break;
			}
			connectedStones.emplace_back(tempGrid);
		}
		break;
	case right:
		for (int j = 0; j < GetWidth() - grid.xPos() + 1; j++)
		{
			Grid gridBase = Grid(grid.GetStoneType(), grid.xPos() + j, grid.yPos());
			if (!TryGetGrid(gridBase, right, tempGrid))
			{
				connectedStones.clear();
				break;
			}
			if (tempGrid.GetStoneType() != enemyType)
			{
				CheckIfGridNone(j, grid, tempGrid, connectedStones);
				break;
			}
			connectedStones.emplace_back(tempGrid);
		}
		break;
	case upperLeft:
		for (int j = 0; j < (grid.yPos() > grid.xPos() ? grid.yPos() : grid.xPos()); j++)
		{
			Grid gridBase = Grid(grid.GetStoneType(), grid.xPos() - j, grid.yPos() - j);
			if (!TryGetGrid(gridBase, upperLeft, tempGrid))
			{
				connectedStones.clear();
				break;
			}
			if (tempGrid.GetStoneType() != enemyType)
			{
				CheckIfGridNone(j, grid, tempGrid, connectedStones);
				break;
			}
			connectedStones.emplace_back(tempGrid);
		}
		break;
	case upperRight:
		for (int j = 0; j < (grid.yPos() > GetWidth() - grid.xPos() + 1 ? grid.yPos() : GetWidth() - grid.xPos() + 1); j++)
		{
			Grid gridBase = Grid(grid.GetStoneType(), grid.xPos() + j, grid.yPos() - j);
			if (!TryGetGrid(gridBase, upperRight, tempGrid))
			{
				connectedStones.clear();
				break;
			}
			if (tempGrid.GetStoneType() != enemyType)
			{
				CheckIfGridNone(j, grid, tempGrid, connectedStones);
				break;
			}
			connectedStones.emplace_back(tempGrid);
		}
		break;
	case lowerLeft:
		for (int j = 0; j < (GetHeight() - grid.yPos() + 1 > grid.xPos() ? GetHeight() - grid.yPos() + 1 : grid.xPos()); j++)
		{
			Grid gridBase = Grid(grid.GetStoneType(), grid.xPos() - j, grid.yPos() + j);
			if (!TryGetGrid(gridBase, lowerLeft, tempGrid))
			{
				connectedStones.clear();
				break;
			}
			if (tempGrid.GetStoneType() != enemyType)
			{
				CheckIfGridNone(j, grid, tempGrid, connectedStones);
				break;
			}
			connectedStones.emplace_back(tempGrid);
		}
		break;
	case lowerRight:
		for (int j = 0; j < (GetHeight() - grid.yPos() > GetWidth() - grid.xPos() ? GetHeight() - grid.yPos() + 1: GetWidth() - grid.xPos() + 1); j++)
		{
			Grid gridBase = Grid(grid.GetStoneType(), grid.xPos() + j, grid.yPos() + j);
			if (!TryGetGrid(gridBase, lowerRight, tempGrid))
			{
				connectedStones.clear();
				break;
			}
			if (tempGrid.GetStoneType() != enemyType)
			{
				CheckIfGridNone(j, grid, tempGrid, connectedStones);
				break;
			}
			connectedStones.emplace_back(tempGrid);
		}
		break;
	default:
		break;
	}
	return connectedStones;
}

std::vector<Grid> const Board::SearchPlaceableGrids(Grid::stoneType type)// マスごとのひっくり返せるマスを設定
{
	std::vector<Grid> flippableGrids;
	std::vector<Grid> placeableGrids;
	for (int i = 0; i < GetHeight(); i++)
	{
		for (int j = 0; j < GetWidth(); j++)
		{
			if (grids[i][j].GetConnectedStones().size() > 0)
			{
				grids[i][j].GetConnectedStones().clear();
			}
			if (grids[i][j].GetStoneType() == Grid::none)
			{
				for (int d = up; d < max; d++)
				{
					std::vector<Grid> searchGrids = SearchFlippableGrids(grids[i][j], (direction)d, type);// ひっくり返せるマスを調べる
					for (int k = 0; k < searchGrids.size(); k++)
					{
						flippableGrids.emplace_back(searchGrids[k]);
					}
				}
				if (flippableGrids.size() > 0)
				{
					grids[i][j].SetConnectedStones(flippableGrids);// 一緒にひっくり返せるマスを設定
					placeableGrids.emplace_back(grids[i][j]);
				}
				flippableGrids.clear();
			}
		}
	}
	return placeableGrids;
}

void Board::PlaceAndFlipStones(Grid grid, Player& player)
{
	grids[grid.yPos() - 1][ grid.xPos() - 1].SetStoneType(player.GetPlayerStoneType());
	for (int i = 0; i < grid.GetConnectedStones().size(); i++)
	{
		if (grids[grid.GetConnectedStones()[i].yPos() - 1][grid.GetConnectedStones()[i].xPos() - 1].GetStoneType() == Grid::white)
		{
			grids[grid.GetConnectedStones()[i].yPos() - 1][grid.GetConnectedStones()[i].xPos() - 1].SetStoneType(Grid::black);
		}
		else
		{
			grids[grid.GetConnectedStones()[i].yPos() - 1][grid.GetConnectedStones()[i].xPos() - 1].SetStoneType(Grid::white);
		}
	}
}

void Board::SetStonesByInput(Player& player, bool& enteredExit, Grid& outPutGrid, bool& autoPlace)// プレイヤーの入力で設置する場所を決める
{
	while (true)
	{
		std::cout << "\n--- 設置可能な箇所の一覧 ---\n";
		for (int i = 0; i < player.GetPlaceableGrids().size(); i++)
		{
			std::cout << i + 1 << ": (縦:" << player.GetPlaceableGrids()[i].yPos() << "横:" << player.GetPlaceableGrids()[i].xPos() << ")\n";
		}
		std::string input;
		int inputNum = 0;
		if (!autoPlace)
		{
			std::cout << "入力(「Exit」で終了、「auto」で自動設置有効化)：";
			std::cin >> input;
			if (!StringToInt(input, inputNum, 5) && input != "Exit" && input != "auto")
			{
				std::cout << "\n!不正な値です。!\n";
				continue;
			}
			if ((inputNum <= 0 || inputNum > player.GetPlaceableGrids().size()) && input != "Exit" && input != "auto")
			{
				std::cout << "\n!入力値は（1～" << player.GetPlaceableGrids().size() << "）の間にしてください。!\n";
				continue;
			}
			if (input == "Exit")
			{
				enteredExit = true;
				break;
			}
			else if (input == "auto")
			{
				std::cout << "!!自動設置を有効化しました。!!\n";
				autoPlace = true;
				inputNum = 1;
			}
		}
		else
		{
			inputNum = 1;
		}
		inputNum--;
		/*std::string inputHorizontal;
		std::cout << "入力(横：「Exit」で終了)：";
		std::cin >> inputHorizontal;
		int inputHorizontalNum = 0;
		if (!StringToInt(inputHorizontal, inputHorizontalNum, 5) && inputHorizontal != "Exit")
		{
			std::cout << "不正な値です。\n";
			continue;
		}
		if ((inputHorizontalNum <= 0 || inputHorizontalNum > GetWidth()) && inputHorizontal != "Exit")
		{
			std::cout << "入力値は（1～" << GetWidth() << "）の間にしてください。\n";
			continue;
		}
		if (inputHorizontal == "Exit")
		{
			enteredExit = true;
			break;
		}*/

		//Grid player1PlaceGrid;
		/*if (!player.TryGetPlaceGrid(inputHorizontalNum, inputNum, player1PlaceGrid))
		{
			std::cout << "その場所には置けません。\n";
			continue;
		}*/

		outPutGrid = player.GetPlaceableGrids()[inputNum];
		PlaceAndFlipStones(player.GetPlaceableGrids()[inputNum], player);
		break;
	}
}

void const Board::ShowBoard()
{
	std::cout << "\n\n";
	std::cout << "    ";
	for (int i = 0; i < GetWidth(); i++)
	{
		std::cout << i + 1 << "  ";
	}
	std::cout << '\n';
	std::cout << "   ------------------------";

	for (int i = 0; i < GetHeight(); i++)
	{
		std::cout << '\n';
		std::cout << i + 1 << " |";
		for (int j = 0; j < GetWidth(); j++)
		{
			if (grids[i][j].GetStoneType() == Grid::white)
			{
				std::cout << "●|";
			}
			else if (grids[i][j].GetStoneType() == Grid::black)
			{
				std::cout << "○|";
			}
			else
			{
				std::cout << "　|";
			}
		}
		std::cout << '\n';
		std::cout << "   ------------------------";
	}
	std::cout << '\n';
	//std::cout << "	●：あなた ○：CPU\n";
}

void const Board::ShowBoard(Player& player)
{
	std::cout << "\n\n";
	std::cout << "    ";
	for (int i = 0; i < GetWidth(); i++)
	{
		std::cout << i + 1 << "  ";
	}
	std::cout << '\n';
	std::cout << "   ------------------------";

	std::vector<Grid> placeableGrids = player.GetPlaceableGrids();
	bool hasHighlighted = false;
	for (int i = 0; i < GetHeight(); i++)
	{
		std::cout << '\n';
		std::cout << i + 1 << " |";
		for (int j = 0; j < GetWidth(); j++)
		{
			if (grids[i][j].GetStoneType() == Grid::white)
			{
				std::cout << "●|";
			}
			else if (grids[i][j].GetStoneType() == Grid::black)
			{
				std::cout << "○|";
			}
			else if (placeableGrids.size() != 0)
			{
				for (int k = 0; k < placeableGrids.size(); k++)
				{
					if (grids[i][j].xPos() == placeableGrids[k].xPos() && grids[i][j].yPos() == placeableGrids[k].yPos())
					{
						std::cout << "＊|";
						placeableGrids.erase(placeableGrids.begin() + k);
						hasHighlighted = true;
					}
				}
			}
			if(!hasHighlighted && grids[i][j].GetStoneType() == Grid::none)
			{
				std::cout << "　|";
			}
			hasHighlighted = false;
		}
		std::cout << '\n';
		std::cout << "   ------------------------";
	}
	std::cout << "	●：あなた ○：CPU|| ＊：置ける場所\n";
}

void const Board::ShowBoard(Player& player, playType type)
{
	std::vector<Grid> placeableGrids = player.GetPlaceableGrids();
	bool hasHighlighted;
	switch (type)
	{
	case serverMode:
		std::cout << "\n\n";
		std::cout << "    ";
		for (int i = 0; i < GetWidth(); i++)
		{
			std::cout << i + 1 << "  ";
		}
		std::cout << '\n';
		std::cout << "   ------------------------";
		hasHighlighted = false;
		for (int i = 0; i < GetHeight(); i++)
		{
			std::cout << '\n';
			std::cout << i + 1 << " |";
			for (int j = 0; j < GetWidth(); j++)
			{
				if (grids[i][j].GetStoneType() == Grid::white)
				{
					std::cout << "●|";
				}
				else if (grids[i][j].GetStoneType() == Grid::black)
				{
					std::cout << "○|";
				}
				else if (placeableGrids.size() != 0)
				{
					for (int k = 0; k < placeableGrids.size(); k++)
					{
						if (grids[i][j].xPos() == placeableGrids[k].xPos() && grids[i][j].yPos() == placeableGrids[k].yPos())
						{
							std::cout << "＊|";
							placeableGrids.erase(placeableGrids.begin() + k);
							hasHighlighted = true;
						}
					}
				}
				if (!hasHighlighted && grids[i][j].GetStoneType() == Grid::none)
				{
					std::cout << "　|";
				}
				hasHighlighted = false;
			}
			std::cout << '\n';
			std::cout << "   ------------------------";
		}
		std::cout << "	●：サーバー（あなた） ○：クライアント|| ＊：置ける場所\n";
		break;
	case clientMode:
		std::cout << '\n';
		std::cout << "    ";
		for (int i = 0; i < GetWidth(); i++)
		{
			std::cout << i + 1 << "  ";
		}
		std::cout << '\n';
		std::cout << "   ------------------------";

		hasHighlighted = false;
		for (int i = 0; i < GetHeight(); i++)
		{
			std::cout << '\n';
			std::cout << i + 1 << " |";
			for (int j = 0; j < GetWidth(); j++)
			{
				if (grids[i][j].GetStoneType() == Grid::white)
				{
					std::cout << "●|";
				}
				else if (grids[i][j].GetStoneType() == Grid::black)
				{
					std::cout << "○|";
				}
				else if (placeableGrids.size() != 0)
				{
					for (int k = 0; k < placeableGrids.size(); k++)
					{
						if (grids[i][j].xPos() == placeableGrids[k].xPos() && grids[i][j].yPos() == placeableGrids[k].yPos())
						{
							std::cout << "＊|";
							placeableGrids.erase(placeableGrids.begin() + k);
							hasHighlighted = true;
						}
					}
				}
				if (!hasHighlighted && grids[i][j].GetStoneType() == Grid::none)
				{
					std::cout << "　|";
				}
				hasHighlighted = false;
			}
			std::cout << '\n';
			std::cout << "   ------------------------";
		}
		std::cout << "	●：サーバー ○：クライアント（あなた）|| ＊：置ける場所\n";
		break;
	default:
		break;
	}
}

bool const Board::CheckGameOver(Player& playerOne, Player& playerTwo)
{
	if (playerOne.GetHasSkipped() && playerTwo.GetHasSkipped())
	{
		return true;
	}
	else
	{
		return false;
	}
}

void const Board::SetStoneTypeNum()
{
	int white = 0, black = 0;
	for (int i = 0; i < GetHeight(); i++)
	{
		for (int j = 0; j < GetWidth(); j++)
		{
			if (grids[i][j].GetStoneType() == Grid::white)
			{
				white++;
			}
			else if (grids[i][j].GetStoneType() == Grid::black)
			{
				black++;
			}
		}
	}
	SetWhiteNum(white);
	SetBlackNum(black);
}

void Board::ResetBoard()
{
	grids.clear();
	for (int i = 0; i < GetHeight(); i++)
	{
		grids.emplace_back();
	}

	for (int h = 0; h < GetHeight(); h++)
	{
		for (int w = 0; w < GetWidth(); w++)
		{
			grids[h].emplace_back(Grid(Grid::none, w + 1, h + 1));
		}
	}
	grids[3][3].SetStoneType(Grid::white);
	grids[4][4].SetStoneType(Grid::white);
	grids[3][4].SetStoneType(Grid::black);
	grids[4][3].SetStoneType(Grid::black);
}

Grid Board::GetGrid(int x, int y)
{
	return grids[y - 1][x - 1];
}

void CheckIfGridNone(int num, Grid grid, Grid tempGrid, std::vector<Grid>& connectedStones)
{
	if (num >= 0 && tempGrid.GetStoneType() == Grid::none)
	{
		connectedStones.clear();
	}
}
bool StringToInt(std::string string, int& num, int maxDigits)
{
	if (string.size() > maxDigits)
	{
		return false;
	}
	int magnification = 1;
	for (int i = 0; i < string.size(); i++)
	{
		int digitNum = string[string.size() - (i + 1)] - '0';
		if (digitNum < 0 || digitNum > 9)
		{
			return false;
		}
		num += digitNum * magnification;
		magnification *= 10;
	}
	return true;
}