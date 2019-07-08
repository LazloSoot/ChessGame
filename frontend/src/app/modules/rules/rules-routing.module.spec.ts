import { RulesRoutingModule } from './rules-routing.module';

describe('RulesRoutingModule', () => {
  let rulesRoutingModule: RulesRoutingModule;

  beforeEach(() => {
    rulesRoutingModule = new RulesRoutingModule();
  });

  it('should create an instance', () => {
    expect(rulesRoutingModule).toBeTruthy();
  });
});
