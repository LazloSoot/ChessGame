import { ProfileSettingsModule } from './profile-settings.module';

describe('ProfileSettingsModule', () => {
  let profileSettingsModule: ProfileSettingsModule;

  beforeEach(() => {
    profileSettingsModule = new ProfileSettingsModule();
  });

  it('should create an instance', () => {
    expect(profileSettingsModule).toBeTruthy();
  });
});
