import { TestBed, inject } from '@angular/core/testing';

import { HomeEventService } from './home-event.service';

describe('HomeEventService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [HomeEventService]
    });
  });

  it('should be created', inject([HomeEventService], (service: HomeEventService) => {
    expect(service).toBeTruthy();
  }));
});
