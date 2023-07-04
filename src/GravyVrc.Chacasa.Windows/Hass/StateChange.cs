namespace GravyVrc.Chacasa.Windows.Hass;

public record struct StateChange(HomeEntity Entity, string NewValue, string OldValue);