import { Game, GameSide, Side, GameStatus } from "../chess";
import { User } from "../user";

export class GameWithConclution {
    isWin?: boolean;
    isDraw?: boolean;
    isResigned?: boolean;
    side: GameSide;
    id: number;
    status: GameStatus;
    creationDate: Date;
    fen: string;
    opponent: User;
    public constructor(
        game: Game,
        targetUserId: number
    ) {
        const targetUserSide = game.sides.find(s => s.id === targetUserId);
        if(targetUserSide) {
            this.initProps(game, targetUserSide);
            const opponentSide = game.sides.find(u => u.id !== targetUserId);
            if(opponentSide) {
                this.opponent = opponentSide.player;
            }
            if(game.status === GameStatus.Completed) {
                this.conclude(opponentSide, targetUserSide);
            }
        }
    }

    private initProps(game: Game, targetUserSide: Side) {
        this.side = targetUserSide.color;
        this.id = game.id;
        this.status = game.status;
        this.creationDate = game.creationDate;
        this.fen = game.fen;
    }

    private conclude(opponentSide: Side, targetUserSide: Side) {
        this.isDraw = targetUserSide.isDraw;
        this.isResigned = targetUserSide.isResign;
        if(this.opponent && targetUserSide) {
            this.isWin = opponentSide.points < targetUserSide.points;
        }
    }
}