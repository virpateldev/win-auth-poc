import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";

import {
  getFileNameFromResponseContentDisposition,
  saveFile
} from "./file-download-helper";

@Injectable()
export class FileService {
  baseUrl = "https://staging.nbcot.org/NbcotApi/api";

  constructor(private http: HttpClient) {}

  buildUrl(endpoint): string {
    return `${this.baseUrl}/${endpoint}`;
  }
  createAuthorizationHeader() {
    const headers = new HttpHeaders();
    const currentUser = JSON.parse(localStorage.getItem("user"));
    const { token } = currentUser;
    return headers.append("Authorization", "Bearer " + token);
  }

  downloadFile(genericAppId: string) {
    const url = "http://localhost:50983/api/File?id=1";
    // Process the file downloaded

    this.http
      .get(url, {
        observe: "response", // to display the full response
        responseType: "blob" as "json"
      })
      .subscribe((res: any) => {
        console.log("res", res);
        const fileName = getFileNameFromResponseContentDisposition(res);
        saveFile(res.body, fileName);
      });
  }

  downloadFile2(genericAppId: string) {
    const url = this.buildUrl(`SmeGenericApplication/resume/${genericAppId}`);
    // Process the file downloaded
    const headers = this.createAuthorizationHeader();
    console.log(headers);

    this.http
      .get(url, {
        headers: headers,
        observe: "response", // to display the full response
        responseType: "blob" as "json"
      })
      .subscribe(res => {
        console.log("res", res);
        const fileName = getFileNameFromResponseContentDisposition(res);
        console.log(fileName);
        // saveFile(res.blob(), fileName);
      });
  }
}
