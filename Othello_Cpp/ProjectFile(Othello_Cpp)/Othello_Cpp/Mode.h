#include<iostream>
#include<WinSock2.h>
#include"Player.h"
#include"Board.h"

#ifndef MODE
#define MODE
class Mode
{
public:
	enum ModeType
	{
		vsCPUMode = 0,
		serverMode = 1,
		clientMode = 2,
		cpuMode = 3
	};

private:
	ModeType type;

public:
	Mode();
	Mode(ModeType type);
	ModeType GetModeType() const { return type; }// é¿çsÉÇÅ[ÉhÇï‘Ç∑
	void SetModeType(ModeType type);
	SOCKET ListenConnect(std::string portInput);
	SOCKET Connect(std::string portInput, const char* serverIP);
	void TurnProcessing(Board& board, Player& player,Player& enemy, SOCKET socket);
	void TurnProcessing(Board& board, Player& player, Player& enemy);
};
#endif // !MODE