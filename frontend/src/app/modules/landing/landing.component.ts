import { Component, OnInit } from '@angular/core';
import { EventService } from '../../shared';

@Component({
  selector: 'app-landing',
  templateUrl: './landing.component.html',
  styleUrls: ['./landing.component.less']
})
export class LandingComponent implements OnInit {

  constructor(private eventService: EventService) { }

  ngOnInit() {
    document.body.classList.add('bg-image');
  }

  onSignInClick() {
    this.eventService.filter('login');
  }

}
