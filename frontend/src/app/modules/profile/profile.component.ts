import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.less']
})
export class ProfileComponent implements OnInit {
  public userProfile : UserProfile;
  constructor() { }

  ngOnInit() {
    this.userProfile = {
      id: 1,
      firstName: "test name",
      lastName: "Test last name",
      birthDate: new Date(1990, 10, 2),
      registrationDate: new Date(2010, 1, 22),
      country: "Ukraine",
      city: "Odessa",
      region: "REGION",
      avatarUrl: 'https://yt3.ggpht.com/a-/AAuE7mCM5HVUnpncyxbI9bbRJ5Hxno7NisiqPT0wVA=s900-mo-c-c0xffffffff-rj-k-no',
      fullName: 'test fullname',
      uid: "uid"
    }
  }

}

export interface UserProfile {
  id?: number;
  firstName?: string;
  lastName?: string;
  birthDate?: Date;
  registrationDate?: Date;
  country?: string;
  city?: string;
  region?: string;
  postalCode?: string;
  address?: string;
  phone?: string;
  avatarUrl?: string;
  fullName?: string;
  uid?: string;
}