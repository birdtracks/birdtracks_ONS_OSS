using System;
using System.Threading.Tasks;

namespace BirdTracks.Game.Core
{
    public interface IService
    {
        bool HasRunInitialize { get; }

        ServiceInitializeResult InitializeResult { get; }

        event Action<ServiceInitializeResult> OnInitialize;

        Task Initialize();
    }
}