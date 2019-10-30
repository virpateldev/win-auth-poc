import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Router } from "@angular/router";

@Injectable({
  providedIn: "root"
})
export class AuthService {
  userData: any;
  baseUrl = "https://staging.nbcot.org/NbcotApi/api";
  localURL = "http://localhost:50983/api";

  constructor(public router: Router, private http: HttpClient) {}

  buildUrl(endpoint): string {
    return `${this.baseUrl}/${endpoint}`;
  }

  SignIn(email: string, password: string) {
    try {
      const payload = {
        Id: 1,
        Email: email,
        ApiUsername: "apiuser",
        ApiPassword: password
      };
      const url = this.buildUrl(`SmeLogin`);
      console.log(url);
      this.http.post<any>(url, payload).subscribe(response => {
        // login successful if there's a jwt token in the response
        if (response) {
          // store user details and jwt token in local storage to keep user logged in between page refreshes
          localStorage.setItem(
            "user",
            JSON.stringify({
              Id: 1,
              Email: email,
              name: "mehulpatel",
              ApiUsername: "apiuser",
              ApiPassword: password,
              token: response
            })
          );
          console.log("login-success", response);
        }
      });
    } catch (error) {
      console.log("error", error);
    }
  }

  ping() {
    const hdr = new HttpHeaders({
      "Content-Type": "application/json"
    });

    this.http
      .get<any>(`${this.localURL}/values/ping`, { headers: hdr })
      .subscribe(
        response => {
          console.log("response", response);
        },
        error => {
          console.log("error", error);
        }
      );
  }

  getUserFromServer() {
    const hdr = new HttpHeaders({ "Content-Type": "application/json" });
    const headers2 = hdr.append("Access-Control-Allow-Credentials", "true");
    this.http
      .get<any>(`${this.localURL}/user/getUser`, {
        headers: headers2,
        withCredentials: true
      })
      .subscribe(
        response => {
          console.log("response", response);
        },
        error => {
          console.log("error", error);
        }
      );
  }

  getUserInfo() {
    // Use access token to retrieve user's profile and set session
    const currentUser = JSON.parse(localStorage.getItem("user"));
    return currentUser;
  }

  get isLoggedIn(): boolean {
    const user = JSON.parse(localStorage.getItem("user"));
    return user !== null;
  }

  async SignOut() {
    localStorage.removeItem("user");
    this.router.navigate(["user/login"]);
  }
}
