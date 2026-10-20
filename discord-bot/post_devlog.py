import os
import sys
import asyncio
import discord
from dotenv import load_dotenv

# Usage: python post_devlog.py "Title of the Dev Log" "Body text or playtest story"

load_dotenv()
TOKEN = os.getenv("DISCORD_BOT_TOKEN")

if not TOKEN:
    print("[Error] DISCORD_BOT_TOKEN not found.")
    sys.exit(1)

if len(sys.argv) < 3:
    print('Usage: python post_devlog.py "<TITLE>" "<MESSAGE>"')
    sys.exit(1)

TITLE = sys.argv[1]
MESSAGE = sys.argv[2].replace("\\n", "\n")

intents = discord.Intents.default()
intents.guilds = True

client = discord.Client(intents=intents)

@client.event
async def on_ready():
    print(f"[Realms of Dust] Dispatcher logged in as {client.user.name}")
    posted = False

    for guild in client.guilds:
        channel = discord.utils.get(guild.text_channels, name="announcements")
        if channel:
            embed = discord.Embed(
                title=f"📢 {TITLE}",
                description=MESSAGE,
                color=discord.Color.gold()
            )
            embed.set_footer(text="Realms of Dust • Announcements & Updates")
            await channel.send(embed=embed)
            print(f"[Success] Posted update to #{channel.name} in {guild.name}")
            posted = True
            break

    if not posted:
        print("[Error] Could not find #dev-log channel in connected servers.")

    await client.close()

if __name__ == "__main__":
    client.run(TOKEN)
