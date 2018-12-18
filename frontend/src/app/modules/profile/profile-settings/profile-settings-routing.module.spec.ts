import { ProfileSettingsRoutingModule } from './profile-settings-routing.module';

describe('ProfileSettingsRoutingModule', () => {
  let profileSettingsRoutingModule: ProfileSettingsRoutingModule;

  beforeEach(() => {
    profileSettingsRoutingModule = new ProfileSettingsRoutingModule();
  });

  it('should create an instance', () => {
    expect(profileSettingsRoutingModule).toBeTruthy();
  });
});
