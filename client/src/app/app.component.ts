import { Component } from "@angular/core";
import { AuthService } from "./services/auth.service";
import { FileService } from "./services/FileService";
@Component({
  selector: "app-root",
  templateUrl: "./app.component.html",
  styleUrls: ["./app.component.css"]
})
export class AppComponent {
  title = "auth";

  constructor(
    public authService: AuthService,
    public fileService: FileService
  ) {}

  onLoginClicked(): void {
    this.authService.SignIn("vpatel@gmail.com", "nbcottest");
  }
  onDownloadFileClicked(): void {
    const genericAppId = "fe2e0708-1ef1-e911-a82a-000d3a1637d3";
    this.fileService.downloadFile(genericAppId);
  }
  onPingClicked(): void {
    console.log("ping-calling");
    this.authService.ping();
  }
  onUserClicked(): void {
    this.authService.getUserFromServer();
  }
}
