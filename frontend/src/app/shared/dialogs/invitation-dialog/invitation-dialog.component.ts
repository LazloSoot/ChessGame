import { Component, OnInit, Inject } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { Invocation, ChessGameService, UserService } from "../../../core";
import { Game, User } from "../../../core";

@Component({
	selector: "app-invitation-dialog",
	templateUrl: "./invitation-dialog.component.html",
	styleUrls: ["./invitation-dialog.component.less"]
})
export class InvitationDialogComponent implements OnInit {
	private inviter: User;
	private game: Game;
	private isUserLoading = true;
	private isGameLoading = true;
	constructor(
		private dialogRef: MatDialogRef<InvitationDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public invitation: Invocation,
		private chessGameService: ChessGameService,
		private userService: UserService
	) {}

	ngOnInit() {
		this.userService.get(this.invitation.inviter.uid).subscribe(inviter => {
			this.inviter = inviter;
			this.isUserLoading = false;
		});
		this.chessGameService.get(this.invitation.gameId).subscribe(game => {
			this.game = game;
			this.isGameLoading = false;
		});
	}
}
