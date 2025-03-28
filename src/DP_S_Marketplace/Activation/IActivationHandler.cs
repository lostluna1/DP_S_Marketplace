namespace DP_S_Marketplace.Activation;

public interface IActivationHandler
{
    bool CanHandle(object args);

    Task HandleAsync(object args);
}
